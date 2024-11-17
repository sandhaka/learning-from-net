using GraphSearch.Graph.Architecture.Abstractions;

namespace GraphSearch.Graph.Architecture.Implementations;

internal class GraphSearch<T>(IGraphComponents<T> graph) : IGraphSearch<T>
{
    private readonly IGraphComponents<T> _graph = graph;

    public bool Search(T target, out SearchResult<T> result)
    {
        throw new NotImplementedException();
    }

    public bool Search(T start, T target, out SearchResult<T> result)
    { 
        // var visited = _graph.NodeValues.Select(v => (Value: v, Visited: false));
        throw new NotImplementedException();
    }
}