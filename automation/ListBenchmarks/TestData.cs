using System.Collections.Immutable;
using LanguageExt;

namespace ListBenchmarks;

public sealed class TestData
{
    public List<int> StandardList { get; } = new();
    public MyImmutableList<int> MyImmutableList { get; }
    public ImmutableList<int> BuiltInImmutableList { get; }
    public Lst<int> LanguageExtList { get; }

    public TestData()
    {
        StandardList.AddRange(Numbers());
        BuiltInImmutableList = ImmutableList<int>.Empty.AddRange(Numbers());
        // reverse list input for my immutable list to get same ordering since implementation has stack semantics
        MyImmutableList = Numbers().Reverse().Aggregate(MyImmutableList<int>.Create(), (l, v) => l.Push(v));
        LanguageExtList = new Lst<int>().AddRange(Numbers());
    }

    private static IEnumerable<int> Numbers() => Enumerable.Range(FirstValue, 1000);

    public const int FirstValue = 0;
    public const int LastValue = 999;
}