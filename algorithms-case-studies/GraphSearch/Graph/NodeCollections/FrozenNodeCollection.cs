using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GraphSearch.Graph.NodeCollections.Abstractions;

namespace GraphSearch.Graph.NodeCollections;

[DebuggerDisplay("{NodesCount} nodes")]
internal sealed class FrozenNodeCollection<T> : NodeCollectionBase<T> where T : class
{
    [SetsRequiredMembers]
    public FrozenNodeCollection(IEnumerable<Node<T>> nodes) : base(nodes.ToFrozenSet())
    {
    }
}