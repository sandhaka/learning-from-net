using GraphSearch.Graph.Architecture.Components;

namespace GraphSearch.Graph.Architecture.Abstractions;

internal interface IGraphComponents<T> 
    where T : IEquatable<T>
{
    IReadOnlySet<T> NodeValues { get; }
    Node<T> this[T value] { get; }
}