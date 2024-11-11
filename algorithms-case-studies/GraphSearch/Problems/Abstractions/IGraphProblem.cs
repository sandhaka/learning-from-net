namespace GraphSearch.Problems.Abstractions;

public interface IGraphProblem<T> where T : class
{
    public IDictionary<T, IEnumerable<T>> Edges { get; }
}