using GraphSearch.Graph.Abstractions;

namespace GraphSearch.Graph;

internal class GraphSearch<T>(IGraph<T> graph) : IGraphSearch<T>
{
    private readonly IGraph<T> _graph = graph;

    public bool Search(T target, out SearchResult<T> result)
    {
        throw new NotImplementedException();
    }

    public bool Search(T start, T target, out SearchResult<T> result)
    {
        throw new NotImplementedException();
    }
}