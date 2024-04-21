using System.Diagnostics.CodeAnalysis;

namespace EventSourcingSourceGeneratorTarget.Models;

internal sealed class ShipHasSailed : PortEvent
{
    [SetsRequiredMembers]
    public ShipHasSailed(Guid shipId, Guid portId)
    {
        ShipId = shipId;
        PortId = portId;
    }
}