using System.Diagnostics.CodeAnalysis;

namespace EventSourcingSourceGeneratorTarget.Models;

internal sealed class ShipHasDocked : PortEvent
{
    [SetsRequiredMembers]
    public ShipHasDocked(DateTime utcDateTime, Guid shipId, Guid portId)
    {
        UtcDateTime = utcDateTime;
        ShipId = shipId;
        PortId = portId;
    }
}