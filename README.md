# Experimentor


``` csharp
public class Program
{
	public static void Main()
	{
        var experiment = new ExperimentBuilder<int>()
            .SetControlBehavior(() => OldSlowMethod())
            .AddCandidateBehavior("NewMethod", () => ShinyNewMethod())
            .AddCandidateBehavior("NewMethod2", () => ShinyNewMethod())
            .Build();

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

	static int OldSlowMethod()
	{
		Thread.Sleep(300);
		return 42;
	}

	static int ShinyNewMethod()
	{
		return 420;
	}
}
```