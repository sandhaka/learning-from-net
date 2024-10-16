using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record LeavedId(Guid Value) : StrongTypedId(Value);


public sealed class Leaved : ISourceEvent
{
    public required LeavedId LeavedId { get; init; }
    public required UserId UserId { get; init; }
    public required DateTime Timestamp { get; init; }
    public required Guid BuildingElementId { get; init; }
    public Guid EventId => LeavedId.Value;
}