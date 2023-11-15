using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

public class Program
{
	public static void Main()
	{
		   var experiment = Scientist.CreateExperiment<int>("Compare Algorithms");

        experiment.Use(() => OldMethod()); // Control
        experiment.Try("NewMethod", () => NewMethod()); // Candidate

        experiment.OnExperimentCompleted += result =>
        {
            Console.WriteLine($"Control took {result.ControlDuration.TotalMilliseconds} ms");
            foreach (var candidate in result.CandidateResults)
            {
                Console.WriteLine($"{candidate.Key} took {candidate.Value.duration.TotalMilliseconds} ms");
            }
        };

        int finalResult = experiment.Run();
	}

	static int OldMethod()
	{
		Thread.Sleep(300);
		return 42;
	}

	static int NewMethod()
	{
		return 420;
	}
}

public interface IExperiment<T>
{
    T Run();
    void Use(Func<T> control);
    void Try(string name, Func<T> candidate);
    event Action<ExperimentResult<T>> OnExperimentCompleted;
}

public class Experiment<T> : IExperiment<T>
{
    private Func<T> _control;
    private Dictionary<string, Func<T>> _candidates = new Dictionary<string, Func<T>>();
    public event Action<ExperimentResult<T>> OnExperimentCompleted;

    public void Use(Func<T> control)
    {
        _control = control;
    }

    public void Try(string name, Func<T> candidate)
    {
        _candidates[name] = candidate;
    }

    public T Run()
    {
        if (_control == null)
            throw new InvalidOperationException("Control function must be provided.");

        var controlResult = ExecuteBehavior(_control, out var controlDuration);

        var candidateResults = new Dictionary<string, (T result, TimeSpan duration)>();
        foreach (var candidate in _candidates)
        {
            var candidateResult = ExecuteBehavior(candidate.Value, out var candidateDuration);
            candidateResults[candidate.Key] = (candidateResult, candidateDuration);
        }

        OnExperimentCompleted?.Invoke(new ExperimentResult<T>
        {
            ControlResult = controlResult,
            ControlDuration = controlDuration,
            CandidateResults = candidateResults
        });

        return controlResult;
    }

    private T ExecuteBehavior(Func<T> behavior, out TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();
        T result = behavior();
        stopwatch.Stop();
        duration = stopwatch.Elapsed;
        return result;
    }
}

public class ExperimentResult<T>
{
    public T ControlResult { get; set; }
    public TimeSpan ControlDuration { get; set; }
    public Dictionary<string, (T result, TimeSpan duration)> CandidateResults { get; set; }
}

public class Scientist
{
    public static IExperiment<T> CreateExperiment<T>(string name)
    {
        return new Experiment<T>();
    }
}

