using GraphSearch.Graph.NodeCollections;

namespace GraphSearch.Graph;

internal sealed class ReadOnly<T> : GraphBase<T>
{
    private ReadOnly(IEnumerable<Node<T>> nodes)
    {
        NodesCollection = new FrozenNodeCollection<T>(nodes);
    }

    public static ReadOnly<T> Create(IEnumerable<Node<T>> nodes)
    {
        var g = new ReadOnly<T>(nodes);
        
        return g;
    }
}