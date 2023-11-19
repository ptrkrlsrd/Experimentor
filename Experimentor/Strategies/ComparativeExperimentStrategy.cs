namespace Experimentor.Strategy;

using System.Diagnostics;
using static Experimentor.Strategies.Constants;

public class ComparativeExperimentStrategy<T> : IExperimentStrategy<T>, IExperimentEventSubscriber<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors;
    public event Action<ExperimentResult<T>>? OnCandidateCompleted;

    internal ComparativeExperimentStrategy(Func<T> controlBehavior, Dictionary<string, Func<T>> candidateBehaviors)
    {
        _controlBehavior = controlBehavior;
        _candidateBehaviors = candidateBehaviors;
    }
    
    public void ExperimentCompleted(Action<ExperimentResult<T>> onCandidateCompleted) => OnCandidateCompleted += onCandidateCompleted;

    private T ExecuteBehavior(Func<T> behavior, out TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();
        T result = behavior();
        
        stopwatch.Stop();
        duration = stopwatch.Elapsed;
        
        return result;
    }

    public ExperimentResult<T> Run()
    {
        T controlResult = ExecuteBehavior(_controlBehavior, out TimeSpan controlExecutionDuration);
        Dictionary<string, CandidateResult<T>> candidateResults = ExecuteAndRecordBehaviors();

        return new ExperimentResult<T>(controlResult, ControlBehaviourName, controlExecutionDuration, candidateResults);
    }

    private Dictionary<string, CandidateResult<T>> ExecuteAndRecordBehaviors()
    {
        IOrderedEnumerable<KeyValuePair<string, Func<T>>> behaviors = _candidateBehaviors.OrderBy(_ => new Random().Next());
        Dictionary<string, CandidateResult<T>> results = new();

        foreach (KeyValuePair<string, Func<T>> behavior in behaviors)
        {
            T result = ExecuteBehavior(behavior.Value, out TimeSpan executionDuration);
            results[behavior.Key] = new(result, executionDuration);
            OnCandidateCompleted?.Invoke(new ExperimentResult<T>(result, behavior.Key, executionDuration));
        }

        return results;
    }
}
