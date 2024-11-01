using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace FrozenCollectionsBmk;

[SimpleJob(RuntimeMoniker.Net80)]
public class CollectionsLookupBmk
{
    private const int Iterations = 10000;
    
    private readonly List<int> _list = Enumerable.Range(0, Iterations).ToList();
    private readonly FrozenSet<int> _frozenSet = Enumerable.Range(0, Iterations).ToFrozenSet();
    private readonly ImmutableList<int> _immutableList = Enumerable.Range(0, Iterations).ToImmutableList();
    private readonly HashSet<int> _hashSet = Enumerable.Range(0, Iterations).ToHashSet();
    private readonly ImmutableHashSet<int> _immutableHashSet = Enumerable.Range(0, Iterations).ToImmutableHashSet();

    [Benchmark(Baseline = true)]
    public void ContainsList() => ContainsWork(_list);
    
    [Benchmark]
    public void ContainsReadOnlyList() => ContainsWork(_list);

    [Benchmark]
    public void ContainsFrozenSet() => ContainsWork(_frozenSet);
    
    [Benchmark]
    public void ContainsImmutableList() => ContainsWork(_immutableList);

    [Benchmark]
    public void ContainsHashSet() => ContainsWork(_hashSet);

    [Benchmark]
    public void ContainsImmutableHashSet() => ContainsWork(_immutableHashSet);

    private static void ContainsWork(IEnumerable<int> sequence)
    {
        for (var i = 0; i < Iterations; i++)
            // ReSharper disable once PossibleMultipleEnumeration
            _ = sequence.Contains(i);
    }
}