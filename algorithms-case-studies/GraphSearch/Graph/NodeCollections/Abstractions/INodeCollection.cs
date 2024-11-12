namespace GraphSearch.Graph.NodeCollections.Abstractions;

internal interface INodeCollection<T> where T : class
{
    int NodesCount { get; }
    bool Contains(T value);
    Node<T> this[T value] { get; }
    IEnumerable<Node<T>> Nodes { get; }
}