using GraphSearch.Graph.NodeCollections;

namespace GraphSearch.Graph;

public class ReadOnlyGraph<T> : Graph<T> where T : class
{
    private readonly IReadOnlyNodeCollection<T> _nodes;
    
    
}