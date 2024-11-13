using GraphSearch.Graph.NodeCollections;

namespace GraphSearch.Graph;

internal sealed class ReadOnlyGraph<T> : GraphBase<T>
{
    private ReadOnlyGraph(IEnumerable<Node<T>> nodes)
    {
        NodesCollection = new FrozenNodeCollection<T>(nodes);
    }

    public static ReadOnlyGraph<T> Create(IEnumerable<Node<T>> nodes)
    {
        var g = new ReadOnlyGraph<T>(nodes);
        
        return g;
    }
}