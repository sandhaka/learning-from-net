using GraphSearch.Graph;
using GraphSearch.Problems.Abstractions;

namespace GraphSearch;

public static class GraphOrchestrator<T> where T : class
{
    public static IGraph<T> CreateReadOnly(IGraphProblem<T> problem)
    {
        CheckProblem(problem);
        
        var nodes = problem.Edges.Keys
            .Select(v => new Node<T>(v))
            .ToHashSet();
        
        SetNeighborhoods(nodes, problem);
        
        var graph = ReadOnlyGraph<T>.Create(nodes);
        
        return graph;
    }

    private static void SetNeighborhoods(HashSet<Node<T>> nodes, IGraphProblem<T> problem)
    {
        for (var i = 0; i < problem.Edges.Count; i++)
        {
            var edge = problem.Edges.ElementAt(i);
            var node = nodes.Single(n => n.Value.Equals(edge.Key));

            var outNodes = edge.Value
                .Select(v => nodes.Single(n => n.Value.Equals(v)))
                .ToArray();

            node.Neighbors = new Memory<Node<T>>(outNodes);
        }
    }

    private static void CheckProblem(IGraphProblem<T> problem)
    {
        // Check if all edges are valid
        var edges = problem.Edges;
        foreach (var edge in edges)
        {
            if (edge.Value.Any(ev => !edges.ContainsKey(ev)))
                throw new InvalidGraphDataException<T> { Item = edge.Key };
        }
    }
}