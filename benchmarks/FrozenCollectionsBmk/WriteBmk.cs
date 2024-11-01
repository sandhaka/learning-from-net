using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using RandomString4Net;

namespace FrozenCollectionsBmk;

[SimpleJob(RuntimeMoniker.Net80)]
public class WriteBmk
{
    private readonly List<string> _names;
    private readonly HashSet<string> _stringHashSet;
    private readonly ImmutableHashSet<string> _stringImmutableHashSet;
    
    public WriteBmk()
    {
        _names = RandomString.GetStrings(Types.ALPHABET_UPPERCASE_WITH_SYMBOLS, 1024);
        _stringHashSet = new HashSet<string>();
        _stringImmutableHashSet = new HashSet<string>().ToImmutableHashSet();
    }

    [Benchmark]
    public void HashSetWrite()
    {
        foreach (var name in _names)
            _ = _stringHashSet.Add(name);
    }
    
    [Benchmark]
    public void ImmutableHashSetWrite()
    {
        foreach (var name in _names)
            _ = _stringImmutableHashSet.Add(name);
    }
}