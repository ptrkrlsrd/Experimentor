using Experimentor.Strategy;
namespace Experimentor.Tests;

public class ExperimentTests
{
    private IExperimentStrategy<int> BuildExperimentStrategy(Func<ExperimentBuilder<int>, ExperimentBuilder<int>> configuration)
    {
        return configuration(new ExperimentBuilder<int>(() => 42)
            .AddCandidate("candidate", () => 69))
            .Build();
    }

    [Fact]
    public void Run_ControlBehaviour()
    {
        // Act
        var builder = BuildExperimentStrategy(b => b.UseComparativeExperimentStrategy());
        ExperimentResult<int> result = builder.Run();

        // Assert
        Assert.Equal(42, result.Result);
        Assert.Equal("control", result.BehaviorName);
    }
    
    [Fact]
    public void Run_UsingSimpleExperimentStrategySelectsCandidate()
    {
        // Act
        var builder = BuildExperimentStrategy(b => b.UseSimpleExperimentStrategy((_, _) => "candidate"));
        ExperimentResult<int> result = builder.Run();

        // Assert
        Assert.Equal(69, result.Result);
        Assert.Equal("candidate", result.BehaviorName);
    }
    
    [Fact]
    public void Run_UsingSimpleExperimentStrategySelectsControl()
    {
        // Act
        var builder = BuildExperimentStrategy(b => b.UseSimpleExperimentStrategy((_, _) => "control"));
        ExperimentResult<int> result = builder.Run();
    
        // Assert
        Assert.Equal(42, result.Result);
        Assert.Equal("control", result.BehaviorName);
    }
    
    [Fact]
    public void Run_RandomSelectionStrategySelectsControlWhen1()
    {
        // Act
        var builder = BuildExperimentStrategy(b => b.UseRandomSelectionStrategy(1));
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
