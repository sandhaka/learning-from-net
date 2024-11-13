namespace GraphSearch.Problems.Abstractions;

public interface IGraphProblem<T>
{
    public IDictionary<T, IEnumerable<(T Value, decimal Cost)>> AdjacencyList { get; }
}