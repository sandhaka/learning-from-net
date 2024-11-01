namespace GraphSearch.Graph;

public static class GraphOrchestrator<T> where T : class
{
    public static Node<T> Load()
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<Node<T>> Search(T start, T target)
    {
        throw new NotImplementedException();
    }
    
    public static IEnumerable<Node<T>> Search(T target)
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<T> Unload()
    {
        throw new NotImplementedException();
    }
}