using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using EventSourcingSourceGeneratorTarget.Option;

namespace EventSourcingSourceGeneratorTarget.Models;

public enum ShipState
{
    UnSet, Docked, Navigating
}

public sealed class Ship
{
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions { WriteIndented = true };

    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required float WeightCapacity { get; init; }
    public ShipState State { get; private set; } = ShipState.UnSet;
    public Guid DockedPortId { get; private set; } = Guid.Empty;

    [SetsRequiredMembers]
    public Ship(string name, float weightCapacity)
    {
        Id = Guid.Empty;
        Name = name;
        WeightCapacity = weightCapacity;
    }
    
    [SetsRequiredMembers]
    public Ship(Guid id, string name, float weightCapacity)
    {
        Id = id;
        Name = name;
        WeightCapacity = weightCapacity;
    }

    public void Sail()
    {
        State = ShipState.Navigating;
        DockedPortId = Guid.Empty;
    }

    public void Dock(Guid portId)
    {
        State = ShipState.Docked;
        DockedPortId = portId;
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