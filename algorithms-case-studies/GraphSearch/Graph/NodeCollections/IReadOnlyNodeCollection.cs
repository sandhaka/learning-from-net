namespace GraphSearch.Graph.NodeCollections;

internal interface IReadOnlyNodeCollection<T> where T : class
{
    bool Contains(T value);
    Node<T> this[T value] { get; }
    ISet<Node<T>> Nodes { get; }
}