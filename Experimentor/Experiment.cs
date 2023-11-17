namespace Experimentor;

public interface IExperimentStrategy<T>
{
    ExperimentResult<T> Run();
}
