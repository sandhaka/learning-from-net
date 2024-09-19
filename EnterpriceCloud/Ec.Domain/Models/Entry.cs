using Ec.Domain.Abstract;

namespace Ec.Domain.Models;

public sealed record EntryId(Guid Value) : StrongTypedId(Value);

/// <summary>
/// Represents an entry in a building system.
/// </summary>
public sealed class Entry : IBuildingElement
{
    public required EntryId EntryId { get; init; }
    public required BuildingId BuildingId { get; init; }
    public required Location Location { get; init; }
    public required string Code { get; init; }
}