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
        
        SetChildren(nodes, problem);
        SetParents(nodes);
        
        var graph = ReadOnlyGraph<T>.Create(nodes);
        
        return graph;
    }

    private static void SetChildren(HashSet<Node<T>> nodes, IGraphProblem<T> problem)
    {
        foreach (var edge in problem.Edges)
        {
            var node = nodes.Single(n => n.Value.Equals(edge.Key));
            
            var children = edge.Value
                .Select(v => nodes.Single(n => n.Value.Equals(v)))
                .ToArray();

            node.Children = null;
            node.Children = new Memory<Node<T>>(children);
        }
    }

    private static void SetParents(HashSet<Node<T>> nodes)
    {
        foreach (var node in nodes)
        {
            var parent = nodes.FirstOrDefault(n =>
            {
                var span = n.Children.Span;
                foreach (var child in span)
                    if (child.Value.Equals(node.Value))
                        return true;
                return false;
            });
            
            node.Parent = parent;
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