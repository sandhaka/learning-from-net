using GraphSearch.Graph.Abstractions;

namespace GraphSearch.Graph.Architecture.Implementations;

internal class GraphSearch<T>(IGraph<T> graph) : IGraphSearch<T>
{
    private readonly IGraph<T> _graph = graph;

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