using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace GraphSearch.Graph.NodeCollections;

internal class FrozenNodeCollection<T> : IEnumerable<Node<T>>, IReadOnlyNodeCollection<T> where T : class
{
    public required IReadOnlyCollection<Node<T>> Nodes { get; init; }

    [SetsRequiredMembers]
    public FrozenNodeCollection(IEnumerable<Node<T>> nodes)
    {
        Nodes = nodes.ToFrozenSet();
    }

    public IEnumerator<Node<T>> GetEnumerator()
    {
        return Nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}