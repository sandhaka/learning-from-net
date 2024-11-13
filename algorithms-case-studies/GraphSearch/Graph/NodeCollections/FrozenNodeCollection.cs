using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GraphSearch.Graph.NodeCollections.Abstractions;

namespace GraphSearch.Graph.NodeCollections;

[DebuggerDisplay("{NodesCount} nodes")]
internal sealed class FrozenNodeCollection<T> : INodeCollection<T>
{
    private readonly FrozenSet<Node<T>> _nodes;
    
    [SetsRequiredMembers]
    public FrozenNodeCollection(IEnumerable<Node<T>> nodes)
    {
        _nodes = nodes.ToFrozenSet();
    }

    public int NodesCount => _nodes.Count;
    
    public bool Contains(T value) => _nodes.Any(node => node.Value.Equals(value));

    public Node<T> this[T value] => _nodes.Single(node => node.Value.Equals(value));
    
    public IReadOnlySet<T> Values => _nodes.Select(n => n.Value).ToHashSet();
}