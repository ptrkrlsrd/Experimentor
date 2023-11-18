namespace Experimentor.Strategy;
using System.Diagnostics;

public class ComparativeExperimentStrategy<T> : IExperimentStrategy<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors;
    public event Action<ExperimentResult<T>>? OnExperimentCompleted;

    internal ComparativeExperimentStrategy(Func<T> controlBehavior, Dictionary<string, Func<T>> candidateBehaviors)
    {
        _controlBehavior = controlBehavior;
        _candidateBehaviors = candidateBehaviors;
    }

    private T ExecuteBehavior(Func<T> behavior, out TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();
        T result = behavior();
        stopwatch.Stop();
        duration = stopwatch.Elapsed;
        return result;
    }

    ExperimentResult<T> IExperimentStrategy<T>.Run()
    {
        T controlResult = ExecuteBehavior(_controlBehavior, out TimeSpan controlExecutionDuration);
        Dictionary<string, (T result, TimeSpan duration)> candidateResults = ExecuteAndRecordBehaviors();
        
        OnExperimentCompleted?.Invoke(new ExperimentResult<T>(controlResult, candidateResults));
        
        return CreateExperimentResult(controlResult, controlExecutionDuration);
    }

    private Dictionary<string, (T result, TimeSpan duration)> ExecuteAndRecordBehaviors()
    {
        IOrderedEnumerable<KeyValuePair<string, Func<T>>> behaviors = _candidateBehaviors.OrderBy(_ => new Random().Next()); 
        Dictionary<string, (T result, TimeSpan duration)> results = new();

        foreach (var behavior in behaviors)
        {
            T result = ExecuteBehavior(behavior.Value, out TimeSpan executionDuration);
            results[behavior.Key] = (result, executionDuration);
        }
    
        return results;
    }

    private ExperimentResult<T> CreateExperimentResult(T controlResult, TimeSpan controlExecutionDuration)
    {
        return new ExperimentResult<T>(controlResult, "control", controlExecutionDuration);
    }
}
