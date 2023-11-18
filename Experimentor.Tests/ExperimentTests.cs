namespace Experimentor.Tests;

using Xunit;
using Experimentor;
using Experimentor.Strategy;

public class ExperimentTests
{
    [Fact]
    public void Run_ControlBehaviour()
    {
        IExperimentStrategy<int> builder = new ExperimentBuilder<int>(() => 42)
            .AddCandidate("candidate", () => 69)
            .UseComparativeExperimentStrategy()
            .Build();

        // Act
        ExperimentResult<int> result = builder.Run();

        // Assert
        Assert.Equal(42, result.Result);
        Assert.Equal("control", result.BehaviorName);
    }
    
    [Fact]
    public void Run_EventControlBehaviour()
    {
        IExperimentStrategy<int> builder = new ExperimentBuilder<int>(() => 42)
            .AddCandidate("candidate", () => 69)
            .UseRandomSelectionStrategy(0.1)
            .Build();

        // Act
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
    
    [Fact]
     public void Run_SelectsBasedOnWeight2()
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
