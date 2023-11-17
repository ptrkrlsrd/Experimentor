using Experimentor.Strategy;
namespace Experimentor;

public class ExperimentBuilder<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors = new();
    private Func<string, List<string>, string> _strategySelector;

    public ExperimentBuilder(Func<T> controlBehavior)
    {
        _controlBehavior = controlBehavior ?? throw new ArgumentNullException(nameof(controlBehavior));
        _strategySelector = (_, _) => "control";
    }

    public ExperimentBuilder<T> AddCandidate(string name, Func<T> candidateBehavior)
    {
        _candidateBehaviors[name] = candidateBehavior ?? throw new ArgumentNullException(nameof(candidateBehavior));
        return this;
    }

    public ExperimentBuilder<T> SetStrategySelector(Func<string, List<string>, string> strategySelector)
    {
        _strategySelector = strategySelector ?? throw new ArgumentNullException(nameof(strategySelector));
        return this;
    }

    public SimpleExperimentStrategy<T> Build()
    {
        return new SimpleExperimentStrategy<T>(_controlBehavior, _candidateBehaviors, _strategySelector);
    }
}
