namespace GraphSearch.Graph.Architecture.NodeCollections.Abstractions;

internal interface IMutableNodeCollection<T> : INodeCollection<T>
    where T : IEquatable<T>
{
    void Add(T value);
    void Remove(T value);
}