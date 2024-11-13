using GraphSearch.Graph;
using GraphSearch.Problems.Abstractions;

namespace GraphSearch;

/// <summary>
/// Orchestrates the creation of graph structures.
/// </summary>
/// <typeparam name="T">The type of the nodes in the graph.</typeparam>
public static class GraphOrchestrator<T>
{
    /// <summary>
    /// Creates a read-only graph from the given problem.
    /// </summary>
    /// <param name="problem">The graph problem containing the adjacency list</param>
    /// <returns>A read-only graph constructed from the problem</returns>
    public static IGraph<T> CreateReadOnly(IGraphProblem<T> problem)
    {
        CheckProblem(problem);
        
        var nodes = problem.AdjacencyList.Keys
            .Select(v => new Node<T>(v))
            .ToHashSet();
        
        SetNeighborhoods(nodes, problem);
        
        var graph = ReadOnlyGraph<T>.Create(nodes);
        
        return graph;
    }

    private static void SetNeighborhoods(HashSet<Node<T>> nodes, IGraphProblem<T> problem)
    {
        for (var i = 0; i < problem.AdjacencyList.Count; i++)
        {
            var (entryKey, neighborValues) = problem.AdjacencyList.ElementAt(i);
            
            var node = nodes.Single(n => n.Value.Equals(entryKey));
            
            var edges = neighborValues
                .Select(x => new Edge<T>(nodes.Single(n => n.Value.Equals(x.Value)), x.Cost))
                .ToArray();
            
            node.Neighbors = new Memory<Edge<T>>(edges);
        }
    }

    private static void CheckProblem(IGraphProblem<T> problem)
    {
        // Check if all edges are valid
        var edges = problem.AdjacencyList;
        foreach (var edge in edges)
        {
            if (edge.Value.Any(ev => !edges.ContainsKey(ev.Value)))
                throw new InvalidGraphDataException<T> { Item = edge.Key };
        }
    }
}