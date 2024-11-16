namespace GraphSearch.Graph.Architecture.Common.PriorityQueue;

internal class PriorityQueue<T>
{
    private readonly SortedSet<PItem<T>> _set = [];
    
    public void Enqueue(T value, decimal priority)
    {
        _set.Add(new PItem<T> { Value = value, Priority = priority });
    }
    
    public T Dequeue()
    {
        var item = _set.Min;
        _set.Remove(item);
        return item.Value;
    }

    public bool TryDequeue(out T value)
    {
        if (_set.Count == 0)
        {
            value = default;
            return false;
        }

        var item = _set.Min;
        _set.Remove(item);
        value = item.Value;
        return true;
    }
}