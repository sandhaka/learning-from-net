using System.Runtime.Serialization;
using System.Text.Json;
using EventSourcingSourceGeneratorTarget.Infrastructure;
using EventSourcingSourceGeneratorTarget.Option;

namespace EventSourcingSourceGeneratorTarget.Models;

internal sealed partial class HarbourMaster(IEventsStore store) : IAggregateRoot
{
    [IgnoreDataMember]
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions { WriteIndented = true };
    
    private readonly IEventsStore _store = store;
    
    // Current state of ports and ships entities
    private readonly HashSet<Port> _ports = [];
    private readonly HashSet<Ship> _ships = [];
    // History
    private readonly IList<PortEvent> _portEvents = [];

    public IOption<Guid> RegisterShip(string shipName, float weightCapacity)
    {
        var ship = new Ship
        {
            Name = shipName ?? throw new ArgumentNullException(nameof(shipName)),
            WeightCapacity = weightCapacity,
            Id = Guid.NewGuid()
        };
        
        if (_ships.Add(ship))
            return new Some<Guid>(ship.Id);
        
        Console.WriteLine($"[WARN]: {ship} not added. Ship {shipName} is just present");
        return new None<Guid>();
    }

    public IOption<Guid> RegisterPort(string portName)
    {
        var port = new Port
        {
            Name = portName ?? throw new ArgumentNullException(nameof(portName)),
            Id = Guid.NewGuid()
        };
        
        if (_ports.Add(port))
            return new Some<Guid>(port.Id);
        
        Console.WriteLine($"[WARN] Harbour Master: {port} not added. Port {portName} is just present");
        return new None<Guid>();
    }
    
    /// <summary>
    /// Load goods on a ship,
    /// let ship leaves the port for navigation to destination
    /// </summary>
    /// <param name="portId">Destination port id</param>
    /// <param name="shipId">Ship id</param>
    public void Sail(Guid portId, Guid shipId)
    {
        var @event = new ShipHasSailed(shipId, portId);
        Apply(@event);
    }

    /// <summary>
    /// Host a ship in a port, unload goods and let it wait for the next trip
    /// </summary>
    /// <param name="portId">Arrival port id</param>
    /// <param name="shipId">Ship id</param>
    public void Dock(Guid portId, Guid shipId)
    {
        var @event = new ShipHasDocked(shipId, portId);
        Apply(@event);
    }
    
    /// <summary>
    /// Locate a ship
    /// </summary>
    /// <param name="shipId">Ship id</param>
    public async void Locate(Guid shipId)
    {
        Ship ship = await GetShipAsync(shipId);
        
        var serialized = JsonSerializer.Serialize(new
        {
            Ship = ship
        }, _jsonSerializerOptions);
        
        Console.WriteLine("[DATA]:");
        Console.WriteLine(serialized);
    }

    private async void Apply(PortEvent @event)
    {
        var ship = await GetShipAsync(@event.ShipId);
        var port = await GetPortAsync(@event.PortId);

        switch (@event)
        {
            case ShipHasSailed:
                ship.Sail();
                Console.WriteLine($"[INFO] Harbour Master: ship {ship.Name} has sailed from {port.Name} port");
                break;
            case ShipHasDocked:
                ship.Dock(@event.PortId);
                Console.WriteLine($"[INFO] Harbour Master: ship {ship.Name} has docked in {port.Name} port");
                break;
        }

        _portEvents.Add(@event);
    }

    // TODO Auto-Build
    public void Save()
    {
        
    }

    // TODO Auto-Build
    public void Load()
    {
        
    }

    // TODO Auto-Build
    private async Task<Ship> GetShipAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    // TODO Auto-Build
    private async Task<Port> GetPortAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}