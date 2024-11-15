namespace GraphSearch.Graph;

public class SearchResult<T>
{
    public required IEnumerable<(T Step, decimal Cost)> Path { get; init; }
    public decimal TotalCost => Path.Sum(x => x.Cost);
}