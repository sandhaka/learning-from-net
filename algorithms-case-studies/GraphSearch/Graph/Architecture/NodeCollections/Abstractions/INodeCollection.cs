using GraphSearch.Graph.Architecture.Components;

namespace GraphSearch.Graph.Architecture.NodeCollections.Abstractions;

internal interface INodeCollection<T>
{
    int NodesCount { get; }
    bool Contains(T value);
    Node<T> this[T value] { get; }
    IReadOnlySet<T> Values { get; }
    IReadOnlySet<Node<T>> Nodes { get; }
}