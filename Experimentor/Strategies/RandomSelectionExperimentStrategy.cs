namespace Experimentor.Strategy;

using System.Diagnostics;
using System.Collections.Generic;

public class RandomSelectionExperimentStrategy<T> : IExperimentStrategy<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors;
    private readonly Random _random;
    private readonly double _controlProbability;

    public RandomSelectionExperimentStrategy(
        Func<T> controlBehavior, 
        Dictionary<string, Func<T>> candidateBehaviors, 
        double controlProbability = 0.5) // Default probability is 0.5 (50%)
    {
        _controlBehavior = controlBehavior ?? throw new ArgumentNullException(nameof(controlBehavior));
        _candidateBehaviors = candidateBehaviors ?? throw new ArgumentNullException(nameof(candidateBehaviors));
        _controlProbability = controlProbability;
        _random = new Random();
    }

    public ExperimentResult<T> Run()
    {
        var stopwatch = Stopwatch.StartNew();
        bool useCandidate = _random.NextDouble() >= _controlProbability;
        
        if (!useCandidate) return ControlExperimentResult();
        
        int candidateIndex = _random.Next(_candidateBehaviors.Count);
        KeyValuePair<string, Func<T>> selectedCandidate = _candidateBehaviors.ElementAt(candidateIndex);
        stopwatch.Stop();
            
        return new ExperimentResult<T>(selectedCandidate.Value(), selectedCandidate.Key, stopwatch.Elapsed);

    }
    private ExperimentResult<T> ControlExperimentResult()
    {
        var stopwatch = Stopwatch.StartNew();
        T resultValue = _controlBehavior();
        const string behaviorName = "control";
        stopwatch.Stop();
        return new ExperimentResult<T>(resultValue, behaviorName, stopwatch.Elapsed);
    }
}
