# Experimentor: A C# Refactoring Library

Experimentor is a versatile C# library designed for refactoring important flows in C# applications. It offers a range of strategies for executing and comparing different code paths. Inspired by Githubs Scientist library for Ruby.

## Getting Started

To integrate Experimentor in your project, add it as a reference in your project file. This enables you to use its functionalities for running experiments using various strategies. (Note: The project is not currently pushed to Nuget). 

## Strategies

Experimentor supports several strategies:

0. **Simple setup default strategy (Comparative Experiment Strategy)**
1. **Comparative Experiment Strategy**
2. **Behaviour Selection Strategyy**
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

## Example

``` csharp
using Experimentor;
using Experimentor.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Define the control behavior: Simple sum of numbers 1 to 1000
        Func<int> controlBehavior = () => Enumerable.Range(1, 1000).Sum();

        // Define a candidate behavior: Complex calculation (e.g., Fibonacci sequence sum for first 30 numbers)
        Func<int> candidateBehavior = () => {
            int a = 0, b = 1, sum = 0;
            for (int i = 0; i < 30; i++)
            {
                sum += a;
                int temp = a;
                a = b;
                b = temp + b;
            }
            return sum;
        };

        // Build the experiment
        var experiment = new ExperimentBuilder<int>(controlBehavior)
            .AddCandidate("candidate1", candidateBehavior)
            .UseComparativeExperimentStrategy()
            .OnExperimentCompleted(result => 
            {
                Console.WriteLine($"Experiment completed. Control: {result.ControlResult}, Candidate: {result.CandidateResults["candidate1"].result}");
            })
            .Build();

        // Run the experiment
        var result = experiment.Run();

        // Output the results
        Console.WriteLine($"Control result: {result.ControlResult}");
        foreach (var candidate in result.CandidateResults)
        {
            Console.WriteLine($"Candidate {candidate.Key} result: {candidate.Value.result}");
        }
    }
}
```

## Benchmark

### Methodology

Tested using a random strategy with 1000 iterations, where 50% of the time the control behavior is selected.
The control behavior is a simple function that performs a bubble sort, while the candidate behavior is a function that performs a quick sort.

### Hardware
* Macbook Pro 2023 (M2 Pro Max)

| Method                  | Mean      | Error    | StdDev   | Gen0   | Allocated |
|------------------------ |----------:|---------:|---------:|-------:|----------:|
| RandomSelectionStrategy | 318.78 us | 3.137 us | 2.934 us | 1.9531 |  16.59 KB |
| RunControlBehaviour     | 551.35 us | 0.587 us | 0.521 us | 1.9531 |  16.09 KB |
| RunCandidateBehaviour   |  77.65 us | 0.108 us | 0.095 us | 1.9531 |  16.09 KB |


## License

Experimentor is licensed under the MIT License. For more details, see the LICENSE file in the repository.
