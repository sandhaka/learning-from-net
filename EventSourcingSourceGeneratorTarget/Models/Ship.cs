using System.Runtime.Serialization;
using System.Text.Json;
using EventSourcingSourceGeneratorTarget.Option;

namespace EventSourcingSourceGeneratorTarget.Models;

internal enum ShipState
{
    UnSet, Docked, Navigating
}

internal sealed class Ship
{
    [IgnoreDataMember]
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions { WriteIndented = true };
    
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required float WeightCapacity { get; init; }
    public ShipState State { get; private set; } = ShipState.UnSet;
    public Option<Guid> PortId { get; private set; } = new None<Guid>();

    public void Sail()
    {
        State = ShipState.Navigating;
        PortId = new None<Guid>();
    }

    public void Dock(Guid portId)
    {
        State = ShipState.Docked;
        PortId = portId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not Ship objShip)
            return false;

        if (objShip.Name == Name)
            return true;

        return false;
    }

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() =>
        JsonSerializer.Serialize(this, _jsonSerializerOptions);
}