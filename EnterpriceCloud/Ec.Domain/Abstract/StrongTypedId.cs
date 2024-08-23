namespace Ec.Domain.Abstract;

public abstract record StrongTypedId(Guid Value) : IComparable<StrongTypedId>
{
    public int CompareTo(StrongTypedId? other) => other is null ? 1 : Value.CompareTo(other.Value);
}