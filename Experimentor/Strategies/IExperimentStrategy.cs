namespace Experimentor;

public interface IExperimentStrategy<T>
{
    ExperimentResult<T> Run();
}

public interface IExperimentEventSubscriber<T>
{
    void ExperimentCompleted(Action<ExperimentResult<T>> onExperimentCompleted);
}
