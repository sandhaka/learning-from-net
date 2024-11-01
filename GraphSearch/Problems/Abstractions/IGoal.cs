namespace GraphSearch.Problems.Abstractions;

public interface IGoal<in T> where T : class
{
    public Func<T, bool> IsReached { get; }
}