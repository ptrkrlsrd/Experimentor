namespace Experimentor.Strategy;

using System.Diagnostics;
using static Experimentor.Strategies.Constants;

public class BehaviorSelectionStrategy<T>(Func<T> controlBehavior, 
                                          Dictionary<string, Func<T>> candidateBehaviors, 
                                          Func<string, List<string>, string> strategySelector) : IExperimentStrategy<T>
{
    public ExperimentResult<T> Run()
    {
        string selectedStrategy = strategySelector(ControlBehaviourName, [..candidateBehaviors.Keys]);
        var stopwatch = Stopwatch.StartNew();

        T result = selectedStrategy == ControlBehaviourName ? controlBehavior() : candidateBehaviors[selectedStrategy]();

        stopwatch.Stop();
        return new ExperimentResult<T>(result, selectedStrategy, stopwatch.Elapsed);
    }
}
