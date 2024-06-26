namespace EventSourcingSourceGenerator.Option;

internal class None<T> : Option<T>
{
    public override T Reduce() => default;
    public override bool IsNone() => true;
}

public sealed class None
{
    private static None Value { get; } = new();
    
    private None() { }
}