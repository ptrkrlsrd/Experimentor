using System.Diagnostics;

public class Experiment<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors;
    public event Action<ExperimentResult<T>>? OnExperimentCompleted;

    internal Experiment(Func<T> controlBehavior, Dictionary<string, Func<T>> candidateBehaviors)
    {
        _controlBehavior = controlBehavior;
        _candidateBehaviors = candidateBehaviors;
    }

    public T Run()
    {
        var controlResult = ExecuteBehavior(_controlBehavior, out var controlDuration);

        var candidateResults = new Dictionary<string, (T result, TimeSpan duration)>();
        foreach (var candidate in _candidateBehaviors)
        {
            var candidateResult = ExecuteBehavior(candidate.Value, out var candidateDuration);
            candidateResults[candidate.Key] = (candidateResult, candidateDuration);
        }

        OnExperimentCompleted?.Invoke(new ExperimentResult<T>
        {
            ControlResult = controlResult,
            ControlDuration = controlDuration,
            CandidateResults = candidateResults
        });

        return controlResult;
    }

    private T ExecuteBehavior(Func<T> behavior, out TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();
        T result = behavior();
        stopwatch.Stop();
        duration = stopwatch.Elapsed;
        return result;
    }
}

