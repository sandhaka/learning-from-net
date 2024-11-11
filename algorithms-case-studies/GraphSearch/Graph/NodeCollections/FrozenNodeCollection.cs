using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics;

namespace GraphSearch.Graph.NodeCollections;

[DebuggerDisplay("{Nodes.Count} nodes")]
internal class FrozenNodeCollection<T> : IEnumerable<Node<T>>, IReadOnlyNodeCollection<T> where T : class
{
    public ISet<Node<T>> Nodes { get; }
    
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
    
    public bool Contains(T value) => Nodes.Any(node => node.Value.Equals(value));
    
    public Node<T> this[T value] => Nodes.Single(node => node.Value.Equals(value));
}