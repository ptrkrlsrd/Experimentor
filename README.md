# Experimentor: A C# Refactoring Library

Experimentor is a versatile C# library designed for refactoring important flows in C# applications. It offers a range of strategies for executing and comparing different code paths.

## Getting Started

To integrate Experimentor in your project, add it as a reference in your project file. This enables you to use its functionalities for running experiments using various strategies.

## Strategies

Experimentor supports several strategies:

0. **Simple setup default strategy (Comparative Experiment Strategy)**
1. **Comparative Experiment Strategy**
2. **Simple Experiment Strategy**
3. **Random Selection Strategy**

Here's how to use each strategy with the Experimentor builder, illustrated with test examples.

### 0. Simple setup default strategy (Comparative Experiment Strategy)

Runs the comparative experiment behavior. This works by running control and the candidate behaviours.

```csharp
var builder = new ExperimentBuilder<int>(() => 42)
    .AddCandidate("candidate", () => 69)
    .Build();
var result = builder.Run();
// Result: Control behavior with value 42
```

### 1. Comparative Experiment Strategy

Runs the control behavior as well as all the candidates.

```csharp
IExperimentStrategy<int>? builder = new ExperimentBuilder<int>(() => 42)
    .AddCandidate("candidate", () => 69)
    .UseComparativeExperimentStrategy()
    .OnCandidateCompleted((r) =>
    {
        Console.WriteLine($"Result: {r.Name} behavior with value {r.Value}");
    })
    .Build();

// Result: Control behavior with value 42
```

### 2. Behaviour Selection Strategy

Selects a specific behavior based on a custom selector.

```csharp
var builder = new ExperimentBuilder<int>(() => 42)
    .AddCandidate("candidate", () => 69)
    .UseBehaviorSelectionStrategy((_, _) => "candidate")
    .Build();
var result = builder.Run();
// Result: Candidate behavior with value 69
```

### 3. Random Selection Strategy

Randomly selects between control and candidate behaviors.

```csharp
var builder = new ExperimentBuilder<int>(() => 42)
    .AddCandidate("candidate", () => 69)
    .UseRandomSelectionStrategy(0.5) // Probability for control
    .Build();
var result = builder.Run();
// Result: Randomly selected behavior
```

## License

Experimentor is licensed under the MIT License. For more details, see the LICENSE file in the repository.
