using GraphSearch.Graph.Architecture.Components;

namespace GraphSearch.Graph.Architecture.Abstractions;

internal interface IGraphComponents<T>
{
    IReadOnlySet<Node<T>> Nodes { get; }
}