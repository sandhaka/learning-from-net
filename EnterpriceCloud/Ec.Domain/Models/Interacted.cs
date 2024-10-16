using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record InteractedId(Guid Value) : StrongTypedId(Value);

public sealed class Interacted : ISourceEvent
{
    public required InteractedId InteractedId { get; init; }
    public required UserId UserId { get; init; }
    public required DateTime Timestamp { get; init; }
    public required Guid BuildingElementId { get; init; }
    public Guid EventId => InteractedId.Value;
}