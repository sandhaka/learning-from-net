using System.Diagnostics;
using Monads.Optional;

namespace GraphSearch.Graph;

[DebuggerDisplay("{Value}")]
internal record Node<T>(T Value) where T : class
{
    private ValueOption<Memory<Node<T>>> _neighbors = ValueOption<Memory<Node<T>>>.None();
    
    public Memory<Node<T>> Neighbors
    {
        get => _neighbors.Reduce(Memory<Node<T>>.Empty);
        set => _neighbors = value.IsEmpty
            ? ValueOption<Memory<Node<T>>>.None()
            : ValueOption<Memory<Node<T>>>.Some(value);
    }
    
    public virtual bool IsEmpty => false;
}

internal record EmptyNode<T>() : Node<T>(default(T)!) where T : class
{
    public override bool IsEmpty => true;
}