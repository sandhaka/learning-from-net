using System.Diagnostics;
using Monads.Optional;

namespace GraphSearch.Graph.Architecture.Components;

[DebuggerDisplay("{Value}")]
internal record Node<T>(T Value) where T : IEquatable<T>
{
    private ValueOption<Memory<Edge<T>>> _neighbors = ValueOption<Memory<Edge<T>>>.None();
    
    public Memory<Edge<T>> Neighbors
    {
        get => _neighbors.Reduce(Memory<Edge<T>>.Empty);
        set => _neighbors = value.IsEmpty
            ? ValueOption<Memory<Edge<T>>>.None()
            : ValueOption<Memory<Edge<T>>>.Some(value);
    }
    
    public bool HasNeighbors => !Neighbors.IsEmpty;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public virtual bool Equals(Node<T> other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (ReferenceEquals(null, other)) return false;
        return Value.Equals(other.Value);
    }
}