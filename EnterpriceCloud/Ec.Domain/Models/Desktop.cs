using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record DesktopId(Guid Value) : StrongTypedId(Value);

/// <summary>
/// Represents a specific desktop.
/// </summary>
public sealed class Desktop : IBuildingElement
{
    public required DesktopId DesktopId { get; init; }
    public required BuildingId BuildingId { get; init; }
    public required Location Location { get; init; }
    public required string Code { get; init; }
}