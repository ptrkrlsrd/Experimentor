public class ExperimentResult<T>
{
    public T ControlResult { get; set; }
    public TimeSpan ControlDuration { get; set; }
    public Dictionary<string, (T result, TimeSpan duration)> CandidateResults { get; set; }
}

