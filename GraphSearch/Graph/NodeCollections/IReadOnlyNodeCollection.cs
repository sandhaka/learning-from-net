namespace GraphSearch.Graph.NodeCollections;

public interface IReadOnlyNodeCollection<T> where T : class
{
    public IReadOnlyCollection<Node<T>> Nodes { get; }
}