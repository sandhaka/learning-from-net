using Ec.Domain.Abstract;
using Ec.Domain.Models.Abstract;

namespace Ec.Domain.Models;

public sealed record BuildingHasReachedId(Guid Value) : StrongTypedId(Value);

public sealed class BuildingHasReached : ISourceEvent
{
    public required BuildingHasReachedId BuildingHasReachedId { get; init; }
    public required UserId UserId { get; init; }
    public required DateTime Timestamp { get; init; }
    public required BuildingId BuildingId { get; init; }
}