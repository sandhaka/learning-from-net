namespace GraphSearch.Graph.Search;

public class SearchResult<T>
{
    public required IEnumerable<T> Path { get; init; }
    public required decimal TotalCost { get; init; }
}