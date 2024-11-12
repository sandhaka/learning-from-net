using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GraphSearch.Graph.NodeCollections.Abstractions;

namespace GraphSearch.Graph.NodeCollections;

[DebuggerDisplay("{NodesCount} nodes")]
internal sealed class MutableNodeCollection<T> : NodeCollectionBase<T>, IMutableNodeCollection<T> where T : class
{
    [SetsRequiredMembers]
    public MutableNodeCollection(IEnumerable<Node<T>> nodes) : base(nodes)
    {
    }

    public void Add(T value)
    {
        throw new NotImplementedException();
    }

    public void Remove(T value)
    {
        throw new NotImplementedException();
    }
}