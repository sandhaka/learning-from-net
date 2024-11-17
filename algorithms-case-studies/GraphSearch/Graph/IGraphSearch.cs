namespace GraphSearch.Graph;

/// <summary>
/// Defines methods for searching within a graph structure.
/// </summary>
/// <typeparam name="T">The type of the elements in the graph nodes.</typeparam>
public interface IGraphSearch<T>
{
    /// <summary>
    /// Searches for a specific target node in the graph.
    /// </summary>
    /// <param name="target">The target node to search for.</param>
    /// <param name="result">
    /// When this method returns, contains the search result,
    /// which includes the path and total cost, if the search is successful.
    /// </param>
    /// <returns>
    /// true if the target node is found; otherwise, false.
    /// </returns>
    bool Search(T target, out SearchResult<T> result);

    /// <summary>
    /// Searches for a specific target node in the graph, starting from the given start node.
    /// </summary>
    /// <param name="start">The node from which to begin the search.</param>
    /// <param name="target">The target node to search for.</param>
    /// <param name="result">
    /// When this method returns, contains the search result,
    /// which includes the path and total cost, if the search is successful.
    /// </param>
    /// <returns>
    /// true if the target node is found; otherwise, false.
    /// </returns>
    bool Search(T start, T target, out SearchResult<T> result);
}