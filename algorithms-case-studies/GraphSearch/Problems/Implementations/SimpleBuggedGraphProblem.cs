using GraphSearch.Problems.Abstractions;

namespace GraphSearch.Problems.Implementations;

public class SimpleBuggedGraphProblem : IGraphProblem<string>
{
    public IDictionary<string, IEnumerable<(string Value, decimal Cost)>> AdjacencyList { get; } = 
        new Dictionary<string, IEnumerable<(string Value, decimal Cost)>> 
        {
            ["A"] = [("B", 1m), ("C", 1m)],
            ["B"] = [("D", 1m)],
            ["C"] = [("K", 1m)],
            ["D"] = [("E", 1m)],
            ["E"] = [("F", 1m)],
            ["F"] = [("G", 1m)],
            ["G"] = [("H", 1m)],
            ["H"] = [("I", 1m)],
            ["I"] = [("J", 1m)],
            ["J"] = [],
            ["K"] = [("L", 1m)],
            ["L"] = [("M", 1m), ("N", 1m)],
            ["N"] = []
        };
}