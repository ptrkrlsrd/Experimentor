using BenchmarkDotNet.Attributes;
namespace Experimentor.Benchmarks;

[MemoryDiagnoser]
public class MemoryBenchmark
{
    [Benchmark]
    public void RandomSelectionStrategy()
    {
        IExperimentStrategy<int[]> builder = new ExperimentBuilder<int[]>(_controlBehavior)
            .AddCandidate("candidate", _candidateBehavior)
            .UseRandomSelectionStrategy(0.5) // Probability for control
            .Build();
        ExperimentResult<int[]> result = builder.Run();
    }
    
    [Benchmark]
    public void RunControlBehaviour()
    {
        _controlBehavior();
    }
    
    [Benchmark]
    public void RunCandidateBehaviour()
    {
        _candidateBehavior();
    }

    private readonly Func<int[]> _controlBehavior = () =>
    {
        int[] data = GenerateRandomArray(1000);
        BubbleSort(data);
        return data;
    };

    private readonly Func<int[]> _candidateBehavior = () =>
    {
        int[] data = GenerateRandomArray(1000);
        QuickSort(data, 0, data.Length - 1);
        return data;
    };

    private static int[] GenerateRandomArray(int size)
    {
        Random rand = new Random();
        return Enumerable.Range(1, size).OrderBy(x => rand.Next()).ToArray();
    }

    static void BubbleSort(int[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                if (array[j] > array[j + 1])
                {
                    (array[j], array[j + 1]) = (array[j + 1], array[j]);
                }
            }
        }
    }

    private static void QuickSort(int[] array, int low, int high)
    {
        if (low >= high) return;
        
        int partition = Partition(array, low, high);
        QuickSort(array, low, partition - 1);
        QuickSort(array, partition + 1, high);
    }

    private static int Partition(int[] array, int low, int high)
    {
        int pivot = array[high];
        int i = (low - 1);
        for (int j = low; j < high; j++)
        {
            if (array[j] >= pivot) continue;
            
            i++;
            (array[i], array[j]) = (array[j], array[i]);
        }
        (array[i + 1], array[high]) = (array[high], array[i + 1]);
        return i + 1;
    }
}
