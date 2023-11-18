using Experimentor.Strategy;
using static Experimentor.Strategies.Constants;

namespace Experimentor.Tests;

public class ExperimentTests
{
    [Fact]
    public void Run_ControlBehaviour()
    {
        // Act

        ExperimentResult<int> candidateResults = default;
        IExperimentStrategy<int>? builder = new ExperimentBuilder<int>(() => 42)
            .AddCandidate("candidate", () => 69)
            .UseComparativeExperimentStrategy()
            .OnExperimentCompleted((r) =>
            {
                candidateResults = r;
            })
            .Build();
        ExperimentResult<int> result = builder.Run();
        // Assert
        Assert.Equal(42, result.Result);
        Assert.Equal("candidate", candidateResults?.BehaviorName);
        Assert.Equal("control", result.BehaviorName);
    }

    [Fact]
    public void Run_UsingSimpleExperimentStrategySelectsCandidate()
    {
        // Act
        IExperimentStrategy<int>? builder = new ExperimentBuilder<int>(() => 42)
            .AddCandidate("candidate", () => 69)
            .UseSimpleExperimentStrategy((_, _) => "candidate")
            .Build();
        ExperimentResult<int> result = builder.Run();
        // Assert
        Assert.Equal(69, result.Result);
        Assert.Equal("candidate", result.BehaviorName);
    }

    [Fact]
    public void Run_UsingSimpleExperimentStrategySelectsControl()
    {
        // Act
        IExperimentStrategy<int>? builder = new ExperimentBuilder<int>(() => 42)
            .AddCandidate("candidate", () => 69)
            .UseSimpleExperimentStrategy((_, _) => "control")
            .Build();
        ExperimentResult<int> result = builder.Run();

        // Assert
        Assert.Equal(42, result.Result);
        Assert.Equal("control", result.BehaviorName);
    }

    [Fact]
    public void Run_RandomSelectionStrategySelectsControlWhen1()
    {
        // Act
        IExperimentStrategy<int>? builder = new ExperimentBuilder<int>(() => 42)
            .AddCandidate("candidate", () => 69)
            .UseRandomSelectionStrategy(1)
            .Build();

        ExperimentResult<int> result = builder.Run();
        // Assert
        Assert.Equal(42, result.Result);
        Assert.Equal("control", result.BehaviorName);
    }

    [Fact]
    public void Run_RandomlySelectsControlOrCandidate()
    {
        // Arrange
        int ControlBehavior() => 1;

        Dictionary<string, Func<int>> candidateBehaviors = new()
        {
            { "candidate1", () => 2 }
        };
        RandomSelectionExperimentStrategy<int> strategy = new(ControlBehavior, candidateBehaviors);
        // Act
        ExperimentResult<int> result = strategy.Run();
        // Assert
        Assert.Contains(result.BehaviorName, new[] { "control", "candidate1" });
    }

    [Fact]
    public void Run_SelectsBasedOnWeight()
    {
        // Arrange
        int ControlBehavior() => 1;
        Dictionary<string, Func<int>> candidateBehaviors = new()
        {
            { "candidate1", () => 2 }
        };
        var controlWeight = 0; // 0% chance to select control
        RandomSelectionExperimentStrategy<int> strategy = new(ControlBehavior, candidateBehaviors, controlWeight);
        // Act & Assert
        ExperimentResult<int> result = strategy.Run();
        Assert.Equal("candidate1", result.BehaviorName);
        Assert.Equal(2, result.Result);
    }
}
