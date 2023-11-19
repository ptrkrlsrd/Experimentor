namespace Experimentor;

public class ExperimentResult<T>
{
    public T Result { get; }
    public string BehaviorName { get; }
    public TimeSpan Duration { get; }
    public Dictionary<string, (T result, TimeSpan duration)> CandidateResults { get; set; }
    
    public ExperimentResult(T result, string behaviorName, TimeSpan duration, Dictionary<string, (T result, TimeSpan duration)>? candidateResults = default)
    {
        Result = result;
        BehaviorName = behaviorName;
        Duration = duration;
        CandidateResults = candidateResults;
    }
}
