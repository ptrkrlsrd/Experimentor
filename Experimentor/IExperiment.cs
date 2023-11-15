public interface IExperiment<T>
{
    T Run();
    void Use(Func<T> control);
    void Try(string name, Func<T> candidate);
    event Action<ExperimentResult<T>> OnExperimentCompleted;
}

