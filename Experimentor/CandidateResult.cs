namespace Experimentor;

public readonly struct CandidateResult<T>(T result, TimeSpan duration)
{
    public T Result { get; } = result;
    public TimeSpan Duration { get; } = duration;
}
