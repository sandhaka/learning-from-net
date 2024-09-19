using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record BuildingId(Guid Value) : StrongTypedId(Value);

/// <summary>
/// Represents a Building.
/// </summary>
public sealed class Building
{
    public required BuildingId BuildingId { get; init; }
    public required int Floors { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string Zip { get; init; }
    public required string Country { get; init; }
}