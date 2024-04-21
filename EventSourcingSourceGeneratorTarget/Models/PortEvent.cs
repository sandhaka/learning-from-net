namespace EventSourcingSourceGeneratorTarget.Models;

internal abstract class PortEvent : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime UtcDateTime { get; } = DateTime.UtcNow;
    public required Guid ShipId { get; init; }
    public required Guid PortId { get; init; }
}