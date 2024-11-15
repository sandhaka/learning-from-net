using GraphSearch.Problems.Abstractions;

namespace GraphSearch.Problems.Samples;

public class SimpleGraphProblem : IGraphProblem<string>
{
    public IDictionary<string, IEnumerable<(string Value, decimal Cost)>> AdjacencyList { get; } = 
        new Dictionary<string, IEnumerable<(string Value, decimal Cost)>> 
        {
            ["A"] = [("B", 0), ("C", 0)],
            ["B"] = [("D", 0)],
            ["C"] = [("K", 0)],
            ["D"] = [("E", 0)],
            ["E"] = [("F", 0)],
            ["F"] = [("G", 0)],
            ["G"] = [("H", 0)],
            ["H"] = [("I", 0)],
            ["I"] = [("J", 0)],
            ["J"] = [],
            ["K"] = [("L", 0)],
            ["L"] = [("M", 0), ("N", 0)],
            ["M"] = [],
            ["N"] = []
        };
}