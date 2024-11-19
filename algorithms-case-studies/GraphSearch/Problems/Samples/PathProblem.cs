using GraphSearch.Problems.Abstractions;

namespace GraphSearch.Problems.Samples;

public class PathProblem : IGraphProblem<string>
{
    public IDictionary<string, IEnumerable<(string Value, decimal Cost)>> AdjacencyList { get; } =
        new Dictionary<string, IEnumerable<(string Value, decimal Cost)>>
        {
            ["A"] = [("B", 3), ("F", 7)],
            ["B"] = [("C", 2), ("H", 8)],
            ["F"] = [("G", 1)],
            ["C"] = [("F", 1)],
            ["H"] = [("K", 1)],
            ["G"] = [("H", 9)],
            ["K"] = [("Z", 0)],
            ["Z"] = []
        };
}