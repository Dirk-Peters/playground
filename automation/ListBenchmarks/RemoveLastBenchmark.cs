using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt;

namespace ListBenchmarks;

public class RemoveLastBenchmark
{
    private TestData testData;

    [IterationSetup]
    public void Setup() => testData = new TestData();

    [Benchmark]
    public void RemoveLastStandard() =>
        testData.StandardList.Remove(TestData.LastValue);

    [Benchmark]
    public MyImmutableList<int> RemoveLastFromMyImmutable() =>
        testData.MyImmutableList.RemoveValues(TestData.LastValue);

    [Benchmark]
    public ImmutableList<int> RemoveLastBuiltIn() =>
        testData.BuiltInImmutableList.Remove(TestData.LastValue);

    [Benchmark]
    public Lst<int> RemoveLastLanguageExt() =>
        testData.LanguageExtList.Remove(TestData.LastValue);
}