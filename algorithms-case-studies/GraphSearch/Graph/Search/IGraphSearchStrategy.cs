using Monads.Optional;

namespace GraphSearch.Graph.Search;

/// <summary>
/// Strategy interface for searching paths within a graph.
/// </summary>
/// <typeparam name="T">The type of node values within the graph.</typeparam>
public interface IGraphSearchStrategy<T>
{
    /// <summary>
    /// Executes the graph search algorithm based on the implemented strategy.
    /// </summary>
    /// <typeparam name="T">The type of node values within the graph.</typeparam>
    /// <param name="context">The context that provides necessary data for the graph search operation.</param>
    /// <param name="result">SearchResult containing the path and the total cost if the search is successful.</param>
    /// <returns>True if the search finds a path to the target node; otherwise, false.</returns>
    bool Run(IPathSearchContext<T> context, out Option<SearchResult<T>> result);

    /// <summary>
    /// Gets the name of the graph search strategy.
    /// </summary>
    string Name { get; }
}