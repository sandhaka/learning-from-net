using GraphSearch.Graph.Architecture.Abstractions;
using GraphSearch.Graph.Search;
using Monads.Optional;

namespace GraphSearch.Graph.Architecture.Implementations;

internal class PathSearch<T>(IGraphComponents<T> graph) : IPathSearch<T>
    where T : IEquatable<T>
{
    // Using Dijkstra as default search strategy
    public IGraphSearchStrategy<T> GraphSearchStrategy { get; set; } = new Dijkstra<T>();

    public bool Search(T start, T target, out Option<SearchResult<T>> result)
    {
        var context = new PathSearchContext<T>(graph, start, target);
        return GraphSearchStrategy.Run(context, out result);
    }
}