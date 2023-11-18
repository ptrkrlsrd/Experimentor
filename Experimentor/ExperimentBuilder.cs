using Experimentor.Strategy;
using System;
using System.Collections.Generic;

namespace Experimentor;

public class ExperimentBuilder<T>
{
    private readonly Func<T> _controlBehavior;
    private readonly Dictionary<string, Func<T>> _candidateBehaviors = new();
    private IExperimentStrategy<T>? _selectedStrategy;
    private bool _strategySet;

    public ExperimentBuilder(Func<T> controlBehavior)
    {
        _controlBehavior = controlBehavior ?? throw new ArgumentNullException(nameof(controlBehavior));
    }

    public ExperimentBuilder<T> AddCandidate(string name, Func<T> candidateBehavior)
    {
        _candidateBehaviors[name] = candidateBehavior ?? throw new ArgumentNullException(nameof(candidateBehavior));
        return this;
    }

    private void SetStrategy(IExperimentStrategy<T> strategy)
    {
        if (_strategySet)
        {
            throw new InvalidOperationException("A strategy has already been set for this experiment.");
        }

        _selectedStrategy = strategy;
        _strategySet = true;
    }

    public ExperimentBuilder<T> UseRandomSelectionStrategy(double controlProbability = 0.5)
    {
        SetStrategy(new RandomSelectionExperimentStrategy<T>(_controlBehavior, _candidateBehaviors, controlProbability));
        return this;
    }

    public ExperimentBuilder<T> UseComparativeExperimentStrategy()
    {
        SetStrategy(new ComparativeExperimentStrategy<T>(_controlBehavior, _candidateBehaviors));
        return this;
    }

    public ExperimentBuilder<T> UseSimpleExperimentStrategy(Func<string, List<string>, string> strategySelector)
    {
        SetStrategy(new SimpleExperimentStrategy<T>(_controlBehavior, _candidateBehaviors, strategySelector));
        return this;
    }

    public IExperimentStrategy<T> Build()
    {
        return _selectedStrategy ?? new RandomSelectionExperimentStrategy<T>(_controlBehavior, _candidateBehaviors);
    }
}
