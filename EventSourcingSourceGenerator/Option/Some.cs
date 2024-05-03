namespace EventSourcingSourceGenerator.Option;

internal class Some<T> : Option<T>
{
    public Some(T item)
    {
        Item = item;
    }
    
    public T Item { get; }
    
    public static implicit operator T(Some<T> some) =>
        some.Item;

    public override T Reduce() => Item;
    public override bool IsNone() => false;
}