using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using RandomString4Net;

namespace FrozenCollectionsBmk;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser(displayGenColumns: false)]
public class ConstructionBmk
{
    private readonly List<string> _names;
    // private readonly HashSet<string> _stringHashSet;
    // private readonly ImmutableHashSet<string> _stringImmutableHashSet;
    // private readonly FrozenSet<string> _stringFrozenSet;
    
    public ConstructionBmk()
    {
        _names = RandomString.GetStrings(Types.ALPHABET_UPPERCASE_WITH_SYMBOLS, 1024);
        
        // _stringHashSet = new HashSet<string>(_names);
        // _stringFrozenSet = _names.ToFrozenSet();
        // _stringImmutableHashSet = _names.ToImmutableHashSet();
    }

    [Benchmark]
    public HashSet<string> HashSetConstruction() => new HashSet<string>(_names);

    [Benchmark]
    public ImmutableHashSet<string> ImmutableSetConstruction() => _names.ToImmutableHashSet();

    [Benchmark]
    public FrozenSet<string> FrozenSetConstruction() => _names.ToFrozenSet();
}