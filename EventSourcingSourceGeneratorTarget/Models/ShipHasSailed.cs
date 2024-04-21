using System.Diagnostics.CodeAnalysis;

namespace EventSourcingSourceGeneratorTarget.Models;

internal sealed class ShipHasSailed : PortEvent
{
    [SetsRequiredMembers]
    public ShipHasSailed(DateTime utcDateTime, Guid shipId, Guid portId)
    {
        UtcDateTime = utcDateTime;
        ShipId = shipId;
        PortId = portId;
    }
}