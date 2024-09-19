using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record ElevatorId(Guid Value) : StrongTypedId(Value);

/// <summary>
/// Represents an elevator within a building.
/// </summary>
public sealed class Elevator : IBuildingElement
{
    public required ElevatorId ElevatorId { get; init; }
    public required BuildingId BuildingId { get; init; }
    public required Location Location { get; init; }
    public required string Code { get; init; }
}