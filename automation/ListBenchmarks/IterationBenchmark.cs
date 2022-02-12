using BenchmarkDotNet.Attributes;

namespace ListBenchmarks;

public class IterationBenchmark
{
    private readonly TestData testData;
    
    public IterationBenchmark() => testData = new TestData();

    [Benchmark]
    public int StandardList() =>
        testData.StandardList.Sum();

    [Benchmark]
    public int MyImmutableList() =>
        testData.MyImmutableList.Sum();

    [Benchmark]
    public int BuiltInImmutableList() =>
        testData.BuiltInImmutableList.Sum();

    [Benchmark]
    public int LanguageExtLst() =>
        testData.LanguageExtList.Sum();
}