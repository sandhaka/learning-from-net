namespace GraphSearch.Graph.Architecture.Common.PriorityQueue;

public struct PItem<T> : IComparable<PItem<T>>
{
    public T Value { get; set; }
    public decimal Priority { get; set; }

    public int CompareTo(PItem<T> other) => Priority.CompareTo(other.Priority);
}