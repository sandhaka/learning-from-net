namespace GraphSearch.Graph.NodeCollections.Abstractions;

internal interface IMutableNodeCollection<T> : INodeCollection<T> where T : class
{
    void Add(T value);
    void Remove(T value);
}