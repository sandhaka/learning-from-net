using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record LiftId(Guid Value) : StrongTypedId(Value);

/// <summary>
/// Represents an elevator in a building.
/// </summary>
public sealed class Lift : IBuildingElement
{
    public required LiftId LiftId { get; init; }
    public required Location Location { get; init; }
    public required string Code { get; init; }
    public required int DestinationFloor { get; init; }
    public string Description => $"Lift-{Code}";
}