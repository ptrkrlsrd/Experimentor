using Experimentor.Strategy;

namespace Experimentor;

public interface IExperimentBuilder<T>
{
    IExperimentBuilderWithCandidates<T> AddCandidate(string name, Func<T> candidateBehavior);
    IExperimentStrategy<T> Build();
}

public interface IEventSupportedExperimentBuilder<T> : IExperimentBuilder<T>
{
    IEventSupportedExperimentBuilder<T> OnExperimentCompleted(Action<ExperimentResult<T>> onExperimentCompleted);
}

public interface IExperimentBuilderWithCandidates<T> : IExperimentBuilder<T>
{
    IEventSupportedExperimentBuilder<T> UseComparativeExperimentStrategy();
    IExperimentBuilderWithCandidates<T> UseSimpleExperimentStrategy(Func<string, List<string>, string> strategySelector);
    IExperimentBuilderWithCandidates<T> UseRandomSelectionStrategy(double controlProbability);
}

public class ExperimentBuilder<T> : IExperimentBuilder<T>, IExperimentBuilderWithCandidates<T>, IEventSupportedExperimentBuilder<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors = new();
    private Action<ExperimentResult<T>>? _onExperimentCompleted;
    private IExperimentStrategy<T>? _selectedStrategy;

    public ExperimentBuilder(Func<T> controlBehavior)
    {
        _controlBehavior = controlBehavior ?? throw new ArgumentNullException(nameof(controlBehavior));
    }

    public IExperimentBuilderWithCandidates<T> AddCandidate(string name, Func<T> candidateBehavior)
    {
        _candidateBehaviors[name] = candidateBehavior ?? throw new ArgumentNullException(nameof(candidateBehavior));
        return this;
    }

    public IEventSupportedExperimentBuilder<T> UseComparativeExperimentStrategy()
    {
        _selectedStrategy = new ComparativeExperimentStrategy<T>(_controlBehavior, _candidateBehaviors);
        return this;
    }
    
    public IExperimentBuilderWithCandidates<T> UseRandomSelectionStrategy(double controlProbability)
    {
        _selectedStrategy = new RandomSelectionExperimentStrategy<T>(_controlBehavior, _candidateBehaviors, controlProbability);
        return this;
    }

    public IExperimentBuilderWithCandidates<T> UseSimpleExperimentStrategy(Func<string, List<string>, string> strategySelector)
    {
        _selectedStrategy = new SimpleExperimentStrategy<T>(_controlBehavior, _candidateBehaviors, strategySelector);
        return this;
    }

    public IEventSupportedExperimentBuilder<T> OnExperimentCompleted(Action<ExperimentResult<T>> onExperimentCompleted)
    {
        _onExperimentCompleted = onExperimentCompleted;
        return this;
    }

    public IExperimentStrategy<T> Build()
    {
        if (_selectedStrategy is ComparativeExperimentStrategy<T> comparativeStrategy)
        {
            comparativeStrategy?.ExperimentCompleted(_onExperimentCompleted);
        }

        return _selectedStrategy ?? throw new InvalidOperationException("A strategy must be set before building the experiment.");
    }
}
