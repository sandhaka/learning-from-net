namespace EventSourcingSourceGeneratorTarget.Models;

public class PortEvent : IEvent
{
    public required Guid Id { get; init; }
    public required DateTime UtcDateTime { get; init; }
    public required Guid ShipId { get; init; }
    public required Guid PortId { get; init; }
}