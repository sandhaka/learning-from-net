namespace GraphSearch.Problems.Abstractions;

public interface IGraphProblem<T> where T : class
{
    public IGoal<T> Goal { get; }
    public IDictionary<T, IEnumerable<T>> Components { get; }
}