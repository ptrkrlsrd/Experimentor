namespace Experimentor;

public readonly struct CandidateResult<T>(T result, TimeSpan duration)
{
    public T Result { get; } = result;
    public TimeSpan Duration { get; } = duration;
}

public class ExperimentResult<T>
{
    public T Result { get; }
    public string BehaviorName { get; }
    public TimeSpan Duration { get; }
    public Dictionary<string, CandidateResult<T>> CandidateResults { get; set; }
    
    public ExperimentResult(T result, string behaviorName, TimeSpan duration, Dictionary<string, CandidateResult<T>> candidateResults = default)
    {
        Result = result;
        BehaviorName = behaviorName;
        Duration = duration;
        CandidateResults = candidateResults;
    }
}
