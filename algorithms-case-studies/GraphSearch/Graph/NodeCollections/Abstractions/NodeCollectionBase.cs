using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GraphSearch.Graph.NodeCollections.Abstractions;

internal abstract class NodeCollectionBase<T> : IEnumerable<Node<T>>, INodeCollection<T> where T : class
{
    public required IEnumerable<Node<T>> Nodes { get; init; }
    
    [SetsRequiredMembers]
    protected NodeCollectionBase(IEnumerable<Node<T>> nodes)
    {
        Nodes = nodes;
    }
    
    public int NodesCount => Nodes.Count();
    
    public IEnumerator<Node<T>> GetEnumerator()
    {
        return Nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public bool Contains(T value) => Nodes.Any(node => node.Value.Equals(value));

    public Node<T> this[T value] => Nodes.Single(node => node.Value.Equals(value));

}