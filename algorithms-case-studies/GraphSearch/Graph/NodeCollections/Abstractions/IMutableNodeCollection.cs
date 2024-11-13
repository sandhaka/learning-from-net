namespace GraphSearch.Graph.NodeCollections.Abstractions;

internal interface IMutableNodeCollection<T> : INodeCollection<T>
{
    void Add(T value);
    void Remove(T value);
}