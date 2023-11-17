using System.Diagnostics;
namespace Experimentor;

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

        T resultValue;
        string behaviorName;

        if (_random.NextDouble() < _controlProbability)
        {
            resultValue = _controlBehavior();
            behaviorName = "control";
        }
        else
        {
            int candidateIndex = _random.Next(_candidateBehaviors.Count);
            KeyValuePair<string, Func<T>> selectedCandidate = _candidateBehaviors.ElementAt(candidateIndex);
            resultValue = selectedCandidate.Value();
            behaviorName = selectedCandidate.Key;
        }

        stopwatch.Stop();
        return new ExperimentResult<T>(resultValue, behaviorName, stopwatch.Elapsed);
    }
}
