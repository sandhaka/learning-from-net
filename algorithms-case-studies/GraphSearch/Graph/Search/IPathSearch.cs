using Monads.Optional;

namespace GraphSearch.Graph.Search;

public interface IPathSearch<T>
{
    IGraphSearchStrategy<T> GraphSearchStrategy { get; set; }
    bool Search(T start, T target, out Option<SearchResult<T>> result);
}