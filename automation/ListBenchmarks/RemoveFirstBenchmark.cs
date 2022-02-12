using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt;

namespace ListBenchmarks;

public class RemoveFirstBenchmark
{
    private TestData testData;

    [IterationSetup]
    public void Setup() => testData = new TestData();

    [Benchmark]
    public void RemoveFirstStandard() =>
        testData.StandardList.Remove(TestData.FirstValue);

    [Benchmark]
    public MyImmutableList<int> RemoveFirstFromMyImmutable() =>
        testData.MyImmutableList.RemoveValues(TestData.FirstValue);

    [Benchmark]
    public ImmutableList<int> RemoveFirstBuiltIn() =>
        testData.BuiltInImmutableList.Remove(TestData.FirstValue);

    [Benchmark]
    public Lst<int> RemoveFirstLanguageExt() =>
        testData.LanguageExtList.Remove(TestData.FirstValue);
}