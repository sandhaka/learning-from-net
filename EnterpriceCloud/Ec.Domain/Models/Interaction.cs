using Ec.Domain.Abstract;
using Ec.Domain.Models.Abstract;

namespace Ec.Domain.Models;

public sealed record InteractionId(Guid Value) : StrongTypedId(Value);

public sealed class Interaction : ISourceEvent
{
    public required InteractionId InteractionId { get; init; }
    public required UserId UserId { get; init; }
    public required DateTime Timestamp { get; init; }
    public required IBuildingElement BuildingElement { get; init; }
}