using System.Diagnostics.CodeAnalysis;

namespace EventSourcingSourceGeneratorTarget.Models;

internal sealed class ShipHasDocked : PortEvent
{
    [SetsRequiredMembers]
    public ShipHasDocked(Guid shipId, Guid portId)
    {
        ShipId = shipId;
        PortId = portId;
    }
}