namespace Experimentor;

using System.Diagnostics;

public interface IExperimentStrategy<T>
{
    ExperimentResult<T> Run();
}

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
        T controlResult = ExecuteBehavior(_controlBehavior, out TimeSpan controlDuration);

        Dictionary<string, (T result, TimeSpan duration)> candidateResults = new();
        var random = new Random();
        foreach (KeyValuePair<string, Func<T>> candidate in _candidateBehaviors.OrderBy(_ => random.Next()))
        {
            T candidateResult = ExecuteBehavior(candidate.Value, out TimeSpan candidateDuration);
            candidateResults[candidate.Key] = (candidateResult, candidateDuration);
        }

        OnExperimentCompleted?.Invoke(new ExperimentResult<T>(controlResult, candidateResults));

        return new ExperimentResult<T>(controlResult, "control", controlDuration);
    }
}
