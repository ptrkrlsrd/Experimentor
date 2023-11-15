public class ExperimentBuilder<T>
{
    private Func<T> _controlBehavior;
    private Dictionary<string, Func<T>> _candidateBehaviors = new Dictionary<string, Func<T>>();

    public ExperimentBuilder<T> SetControlBehavior(Func<T> control)
    {
        _controlBehavior = control;
        return this;
    }

    public ExperimentBuilder<T> AddCandidateBehavior(string name, Func<T> candidate)
    {
        _candidateBehaviors[name] = candidate;
        return this;
    }

    public Experiment<T> Build()
    {
        if (_controlBehavior == null)
            throw new InvalidOperationException("Control behavior must be set.");

        if (_candidateBehaviors.Count == 0)
            throw new InvalidOperationException("At least one candidate behavior must be set.");

        return new Experiment<T>(_controlBehavior, _candidateBehaviors);
    }
}