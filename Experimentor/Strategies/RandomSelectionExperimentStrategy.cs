using static Experimentor.Strategies.Constants;
using System.Diagnostics;

namespace Experimentor.Strategy;

public class RandomSelectionExperimentStrategy<T>(Func<T> controlBehavior,
                                                  Dictionary<string, Func<T>> candidateBehaviors, 
                                                  double controlProbability = 0.5) : IExperimentStrategy<T>
{

    public ExperimentResult<T> Run()
    {
        bool useCandidate = new Random().NextDouble() >= controlProbability;

        return useCandidate ? CandidateResult() : ControlResult();
    }
    
    private ExperimentResult<T> CandidateResult()
    {
        var stopwatch = Stopwatch.StartNew();
        int candidateIndex = new Random().Next(candidateBehaviors.Count);
        
        KeyValuePair<string, Func<T>> selectedCandidate = candidateBehaviors.ElementAt(candidateIndex);
        stopwatch.Stop();

        T selectedCandidateValue = selectedCandidate.Value();
        Dictionary<string, CandidateResult<T>> candidateResults = new()
        {
            { selectedCandidate.Key,  new(selectedCandidateValue, stopwatch.Elapsed) }
        };
        
        return new ExperimentResult<T>(selectedCandidateValue, selectedCandidate.Key, stopwatch.Elapsed, candidateResults);
    }
    
    private ExperimentResult<T> ControlResult()
    {
        var stopwatch = Stopwatch.StartNew();
        T resultValue = controlBehavior();
        stopwatch.Stop();
        return new ExperimentResult<T>(resultValue, ControlBehaviourName, stopwatch.Elapsed);
    }
}
