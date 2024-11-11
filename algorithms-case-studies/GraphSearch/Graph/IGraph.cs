using Monads.Optional;

namespace GraphSearch.Graph;

public interface IGraph<T>
{
    void Dfs(T start, Option<Action<T>> action);
    ISet<T> NodeValues { get; }
}