namespace EventSourcingSourceGeneratorTarget.Models;

public class PortEvent : IEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required DateTime UtcDateTime { get; init; }
    public required Guid ShipId { get; init; }
    public required Guid PortId { get; init; }
}