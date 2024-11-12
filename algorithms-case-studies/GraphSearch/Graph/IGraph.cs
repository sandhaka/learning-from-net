using Monads.Optional;

namespace GraphSearch.Graph;

/// <summary>
/// Represents a generic graph interface.
/// </summary>
/// <typeparam name="T">The type of the elements in the graph nodes.</typeparam>
public interface IGraph<T>
{
    /// <summary>
    /// Gets or sets an optional action to be performed on each node during graph traversal.
    /// </summary>
    /// <remarks>
    /// This property allows assigning a delegate of type <c>NodeAction&lt;T&gt;</c> which
    /// can be invoked with the node's value as a parameter during operations like Depth-First Search (DFS).
    /// </remarks>
    Option<NodeAction<T>> ActionParameter { get; set; }

    /// <summary>
    /// Performs a depth-first search (DFS) on a graph starting from the specified node.
    /// The method traverses the graph and optionally executes a specified action on each node encountered.
    /// </summary>
    /// <param name="start">The node from which the DFS will begin.</param>
    void Dfs(T start);

    /// <summary>
    /// Gets the set of values contained within the nodes of the graph.
    /// </summary>
    /// <remarks>
    /// This property provides access to the values held by the nodes in the graph.
    /// It can be useful for operations that need to work with the data stored in nodes,
    /// such as traversal or search functionality.
    /// </remarks>
    ISet<T> NodeValues { get; }
}