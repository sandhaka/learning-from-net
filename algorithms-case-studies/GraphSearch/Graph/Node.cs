using System.Diagnostics;
using Monads.Optional;

namespace GraphSearch.Graph;

[DebuggerDisplay("{Value}")]
internal record Node<T>(T Value)
{
    private ValueOption<Memory<Edge<T>>> _neighbors = ValueOption<Memory<Edge<T>>>.None();
    
    public Memory<Edge<T>> Neighbors
    {
        get => _neighbors.Reduce(Memory<Edge<T>>.Empty);
        set => _neighbors = value.IsEmpty
            ? ValueOption<Memory<Edge<T>>>.None()
            : ValueOption<Memory<Edge<T>>>.Some(value);
    }
    
    public virtual bool IsEmpty => false;
    
    public bool HasNeighbors => !Neighbors.IsEmpty;
}