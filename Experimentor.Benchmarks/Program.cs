using BenchmarkDotNet.Running;
namespace Experimentor.Benchmarks;


public class Program 
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<MemoryBenchmark>();
    }
}
