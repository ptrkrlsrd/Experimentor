namespace Experimentor.Strategy;

using System.Diagnostics;

  
public class SimpleExperimentStrategy<T> : IExperimentStrategy<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors;
    private readonly Func<string, List<string>, string> _strategySelector;
    public event Action<ExperimentResult<T>>? OnExperimentCompleted;

    public SimpleExperimentStrategy(Func<T> controlBehavior, Dictionary<string, Func<T>> candidateBehaviors, Func<string, List<string>, string> strategySelector)
    {
        _controlBehavior = controlBehavior;
        _candidateBehaviors = candidateBehaviors;
        _strategySelector = strategySelector;
    }

    public ExperimentResult<T> Run()
    {
        string selectedStrategy = _strategySelector("control", new List<string>(_candidateBehaviors.Keys));
        var stopwatch = Stopwatch.StartNew();

        T result = selectedStrategy == "control" ? _controlBehavior() : _candidateBehaviors[selectedStrategy]();

        stopwatch.Stop();
        return new ExperimentResult<T>(result, selectedStrategy, stopwatch.Elapsed);
    }
}
