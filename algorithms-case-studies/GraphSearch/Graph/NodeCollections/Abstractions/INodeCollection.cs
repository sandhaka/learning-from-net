namespace GraphSearch.Graph.NodeCollections.Abstractions;

internal interface INodeCollection<T>
{
    int NodesCount { get; }
    bool Contains(T value);
    Node<T> this[T value] { get; }
    IReadOnlySet<T> Values { get; }
}