namespace EventSourcingSourceGeneratorTarget.Option;

public class Some<T>(T item) : Option<T>
{
    public T Item { get; } = item;
    
    public static implicit operator T(Some<T> some) =>
        some.Item;

    public override T Reduce() => Item;
    public override bool IsNone() => false;
}