namespace Experimentor;

public class ExperimentResult<T>
{
    public T Result { get; }
    public string BehaviorName { get; }
    public TimeSpan Duration { get; private set; }
    public void SetDuration(TimeSpan duration) => Duration = duration;


    public TimeSpan ControlDuration { get; set; }
    public Dictionary<string, (T result, TimeSpan duration)> CandidateResults { get; set; }

    public ExperimentResult(T result, string behaviorName)
    {
        Result = result;
        BehaviorName = behaviorName;
    }

    public ExperimentResult(T result, string behaviorName, TimeSpan duration)
    {
        Result = result;
        BehaviorName = behaviorName;
        Duration = duration;
    }

    public ExperimentResult(T result, Dictionary<string, (T result, TimeSpan duration)> candidateResults)
    {
        Result = result;
        CandidateResults = candidateResults;
    }
}

