# Experiment Strategies Library

This library provides a flexible way to run experiments in your C# applications. It supports various strategies to control how experiments are executed and compared.

## Getting Started

To use this library, first include it in your project. You can do this by adding a reference to the library in your project file.

## Strategies

The library currently supports the following strategies:

1. **Random Selection Strategy**
2. **Weighted Strategy**
3. **Sequential Strategy**
4. **Control and Candidate Strategy**

Below are examples of how to use each strategy.

### 1. Random Selection Strategy

This strategy randomly selects between the control and candidate behaviors.

```csharp
var controlBehavior = new Func<int>(() => 1);
var candidateBehaviors = new Dictionary<string, Func<int>> { { "candidate1", () => 2 } };
var strategy = new RandomSelectionExperimentStrategy<int>(controlBehavior, candidateBehaviors);

var result = strategy.Run();
Console.WriteLine($"Selected behavior: {result.SelectedBehavior}");
```

### 2. Weighted Strategy

This strategy selects behaviors based on assigned weights, giving more control over the likelihood of each behavior being chosen.

```csharp
var controlBehavior = new Func<int>(() => 1);
var candidateBehaviors = new Dictionary<string, Func<int>> { { "candidate1", () => 2 } };
var controlWeight = 0.7; // 70% chance to select control
var strategy = new WeightedExperimentStrategy<int>(controlBehavior, candidateBehaviors, controlWeight);

var result = strategy.Run();
Console.WriteLine($"Selected behavior: {result.SelectedBehavior}");
```

### 3. Sequential Strategy

This strategy goes through each behavior sequentially.

```csharp
var controlBehavior = new Func<int>(() => 1);
var candidateBehaviors = new Dictionary<string, Func<int>> { { "candidate1", () => 2 }, { "candidate2", () => 3 } };
var strategy = new SequentialExperimentStrategy<int>(controlBehavior, candidateBehaviors);

var result = strategy.Run();
Console.WriteLine($"Selected behavior: {result.SelectedBehavior}");
```

### 4. Control and Candidate Strategy

This strategy runs both control and candidate behaviors, allowing for direct comparison.

```csharp
var controlBehavior = new Func<int>(() => 1);
var candidateBehaviors = new Dictionary<string, Func<int>> { { "candidate1", () => 2 } };
var strategy = new ControlAndCandidateExperimentStrategy<int>(controlBehavior, candidateBehaviors);

var result = strategy.Run();
Console.WriteLine($"Control result: {result.ControlResult}");
foreach (var candidate in result.CandidateResults)
{
    Console.WriteLine($"Candidate {candidate.Key} result: {candidate.Value.result}");
}
```

## Contributing

Contributions to this library are welcome. Please ensure that your code adheres to the existing style and that all tests pass.

## License

This library is licensed under MIT. Please see the LICENSE file for more details.
