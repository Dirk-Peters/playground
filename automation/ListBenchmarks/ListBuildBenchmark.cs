using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using LanguageExt;

namespace ListBenchmarks;

/// <summary>
/// Compare iteratively adding more items to a list.
/// Uses single add on purpose to simulate independent changes
/// instead of creating a list from a known number of items
/// with AddRange. The latter needs a separate benchmark. 
/// </summary>
public class ListBuildBenchmark
{
    [Params(10, 100, 1000)] public int Size { get; set; }

    [Benchmark]
    public List<int> StandardList() =>
        Enumerable.Range(0, Size)
            .Aggregate(new List<int>(), (l, v) =>
            {
                l.Add(v);
                return l;
            });

    [Benchmark]
    public MyImmutableList<int> MyImmutableList() =>
        Enumerable.Range(0, Size)
            .Aggregate(MyImmutableList<int>.Create(), (l, v) => l.Push(v));

    [Benchmark]
    public ImmutableList<int> BuiltInImmutableList() =>
        Enumerable.Range(0, Size)
            .Aggregate(ImmutableList<int>.Empty, (l, v) => l.Add(v));

    [Benchmark]
    public Lst<int> LanguageExtLst() =>
        Enumerable.Range(0, Size)
            .Aggregate(new Lst<int>(), (l, v) => l.Add(v));
}