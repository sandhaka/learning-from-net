namespace GraphSearch.Graph.Parameters;

/// <summary>
/// Delegate to define a method signature for visiting nodes during graph traversal.
/// </summary>
/// <typeparam name="T">The type of the node being visited.</typeparam>
/// <param name="node">The node being visited.</param>
public delegate void OnVisit<in T>(T node);