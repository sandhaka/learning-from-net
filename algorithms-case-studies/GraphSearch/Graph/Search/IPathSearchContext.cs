namespace GraphSearch.Graph.Search;

/// <summary>
/// Represents the context required for path search operations within a graph.
/// </summary>
/// <typeparam name="T">The type of node values within the graph.</typeparam>
public interface IPathSearchContext<T>
{
    /// <summary>
    /// Gets the set of all unique node values within the graph.
    /// </summary>
    /// <typeparam name="T">The type of node values.</typeparam>
    IReadOnlySet<T> NodeValues { get; }

    /// <summary>
    /// Retrieves the neighbors of a specified node and the associated costs.
    /// </summary>
    /// <param name="value">The value of the node for which neighbors are to be retrieved.</param>
    /// <returns>An enumerable list of tuples where each tuple contains a neighboring node's value and the cost associated with moving to that neighbor.</returns>
    IEnumerable<(T Value, decimal Cost)> Neighbors(T value);

    /// <summary>
    /// Gets the starting node value for the path search within the graph.
    /// </summary>
    T Start { get; }

    /// <summary>
    /// Gets the target node value for the search operation.
    /// The search algorithm attempts to find a path from the start node to this target node.
    /// </summary>
    /// <typeparam name="T">The type of node values within the graph.</typeparam>
    T Target { get; }
}