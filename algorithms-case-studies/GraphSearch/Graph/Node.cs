using System.Diagnostics;
using Monads.Optional;

namespace GraphSearch.Graph;

[DebuggerDisplay("{Value}")]
internal record Node<T>(T Value) where T : class
{
    private ValueOption<Memory<Node<T>>> _children = ValueOption<Memory<Node<T>>>.None();
    private Option<Node<T>> _parent = Option<Node<T>>.None();
    
    public Memory<Node<T>> Children
    {
        get => _children.Reduce(Memory<Node<T>>.Empty);
        set => _children = value.IsEmpty
            ? ValueOption<Memory<Node<T>>>.None()
            : ValueOption<Memory<Node<T>>>.Some(value);
    }

    public Node<T> Parent
    {
        get => _parent.Reduce(new EmptyNode<T>());
        set => _parent = value is null 
            ? Option<Node<T>>.None() 
            : Option<Node<T>>.Some(value);
    }

    public virtual bool IsEmpty => false;
}

internal record EmptyNode<T>() : Node<T>(default(T)!) where T : class
{
    public override bool IsEmpty => true;
}