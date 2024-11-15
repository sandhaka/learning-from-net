using Monads.Optional;

namespace GraphSearch.Graph.Abstractions;

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
    /// This property allows assigning a delegate of type <c>OnVisit&lt;T&gt;</c> which
    /// can be invoked with the node's value as a parameter during operations like Depth-First Search (DFS).
    /// </remarks>
    Option<OnVisit<T>> OnVisitActionParameter { get; set; }

    /// <summary>
    /// Performs a depth-first search (DFS) on a graph starting from the specified node.
    /// The method traverses the graph and optionally executes a specified action on each node encountered.
    /// </summary>
    /// <param name="start">The node from which the DFS will begin.</param>
    void TraverseDfs(T start);

    /// <summary>
    /// Performs a breadth-first search (BFS) on a graph starting from the specified node.
    /// The method traverses the graph and optionally executes a specified action on each node encountered.
    /// </summary>
    /// <param name="start">The node from which the BFS will begin.</param>
    void TraverseBfs(T start);

    /// <summary>
    /// Retrieves the set of values contained within the nodes of the graph.
    /// </summary>
    /// <remarks>
    /// This property returns an `ISet` containing the values of all nodes present in the graph's node collection.
    /// It provides a way to access all distinct node values in the graph.
    /// </remarks>
    IReadOnlySet<T> NodeValues { get; }
    
    IGraphSearch<T> ToSearchGraph();
}