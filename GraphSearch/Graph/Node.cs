namespace GraphSearch.Graph;

public record Node<T>(T Value, Memory<Node<T>> Children) where T : class
{
    
}