using Experimentor.Strategy;

namespace Experimentor;

public interface IExperimentBuilder<T>
{
    IExperimentBuilderWithCandidates<T> AddCandidate(string name, Func<T> candidateBehavior);
    IExperimentStrategy<T> Build();
}

public interface IEventSupportedExperimentBuilder<T> : IExperimentBuilder<T>
{
    IEventSupportedExperimentBuilder<T> OnCandidateCompleted(Action<ExperimentResult<T>> OnCandidateCompleted);
}

public interface IExperimentBuilderWithCandidates<T> : IExperimentBuilder<T>
{
    IEventSupportedExperimentBuilder<T> UseComparativeExperimentStrategy();
    IExperimentBuilderWithCandidates<T> UseBehaviorSelectionStrategy(Func<string, List<string>, string> strategySelector);
    IExperimentBuilderWithCandidates<T> UseRandomSelectionStrategy(double controlProbability);
}

public class ExperimentBuilder<T>(Func<T> controlBehavior) : IExperimentBuilderWithCandidates<T>, IEventSupportedExperimentBuilder<T>
{
    private readonly Dictionary<string, Func<T>> _candidateBehaviors = new();
    private Action<ExperimentResult<T>>? _onCandidateCompleted;
    private IExperimentStrategy<T>? _selectedStrategy;

    public IExperimentBuilderWithCandidates<T> AddCandidate(string name, Func<T> candidateBehavior)
    {
        _candidateBehaviors[name] = candidateBehavior ?? throw new ArgumentNullException(nameof(candidateBehavior));
        return this;
    }

    public IEventSupportedExperimentBuilder<T> UseComparativeExperimentStrategy()
    {
        _selectedStrategy = new ComparativeExperimentStrategy<T>(controlBehavior, _candidateBehaviors);
        return this;
    }
    
    public IExperimentBuilderWithCandidates<T> UseRandomSelectionStrategy(double controlProbability)
    {
        _selectedStrategy = new RandomSelectionExperimentStrategy<T>(controlBehavior, _candidateBehaviors, controlProbability);
        return this;
    }

    public IExperimentBuilderWithCandidates<T> UseBehaviorSelectionStrategy(Func<string, List<string>, string> strategySelector)
    {
        _selectedStrategy = new BehaviorSelectionStrategy<T>(controlBehavior, _candidateBehaviors, strategySelector);
        return this;
    }

    public IEventSupportedExperimentBuilder<T> OnCandidateCompleted(Action<ExperimentResult<T>> onCandidateCompleted)
    {
        _onCandidateCompleted = onCandidateCompleted;
        return this;
    }

    public IExperimentStrategy<T> Build()
    {
        if (_selectedStrategy is ComparativeExperimentStrategy<T> comparativeStrategy && _onCandidateCompleted is not null)
        {
            comparativeStrategy.ExperimentCompleted(_onCandidateCompleted);
        }

        return _selectedStrategy ?? throw new InvalidOperationException("A strategy must be set before building the experiment.");
    }
}
