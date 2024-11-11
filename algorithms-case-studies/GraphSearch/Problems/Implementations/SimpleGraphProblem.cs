using GraphSearch.Problems.Abstractions;

namespace GraphSearch.Problems.Implementations;

public class SimpleGraphProblem : IGraphProblem<string>
{
    public IDictionary<string, IEnumerable<string>> Edges { get; } = 
        new Dictionary<string, IEnumerable<string>> 
        {
            ["A"] = ["B", "C"],
            ["B"] = ["D"],
            ["C"] = ["K"],
            ["D"] = ["E"],
            ["E"] = ["F"],
            ["F"] = ["G"],
            ["G"] = ["H"],
            ["H"] = ["I"],
            ["I"] = ["J"],
            ["J"] = [],
            ["K"] = ["L"],
            ["L"] = ["M", "N"],
            ["M"] = [],
            ["N"] = [],
            
        };
}