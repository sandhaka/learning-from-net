namespace GraphSearch.Graph.Abstractions;

public interface IGraphSearch<T>
{
    bool Search(T target, out SearchResult<T> result);
    bool Search(T start, T target, out SearchResult<T> result);
}