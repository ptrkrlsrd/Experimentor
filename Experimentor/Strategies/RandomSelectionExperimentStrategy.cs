using static Experimentor.Strategies.Constants;
using System.Diagnostics;

namespace Experimentor.Strategy;

public class RandomSelectionExperimentStrategy<T>(Func<T> controlBehavior,
                                                  Dictionary<string, Func<T>> candidateBehaviors, 
                                                  double controlProbability = 0.5) : IExperimentStrategy<T>
{
    private readonly Random _random = new();

    public ExperimentResult<T> Run()
    {
        var stopwatch = Stopwatch.StartNew();
        bool useCandidate = _random.NextDouble() >= controlProbability;

        if (!useCandidate) return ControlExperimentResult();

        int candidateIndex = _random.Next(candidateBehaviors.Count);
        KeyValuePair<string, Func<T>> selectedCandidate = candidateBehaviors.ElementAt(candidateIndex);
        stopwatch.Stop();

        return new ExperimentResult<T>(selectedCandidate.Value(), selectedCandidate.Key, stopwatch.Elapsed);

    }
    private ExperimentResult<T> ControlExperimentResult()
    {
        var stopwatch = Stopwatch.StartNew();
        T resultValue = controlBehavior();
        stopwatch.Stop();
        return new ExperimentResult<T>(resultValue, ControlBehaviourName, stopwatch.Elapsed);
    }
}
