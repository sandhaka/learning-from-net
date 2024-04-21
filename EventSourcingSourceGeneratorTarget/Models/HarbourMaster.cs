using System.Text.Json;
using EventSourcingSourceGeneratorTarget.Infrastructure;
using EventSourcingSourceGeneratorTarget.Option;

namespace EventSourcingSourceGeneratorTarget.Models;

internal sealed partial class HarbourMaster : IAggregateRoot
{
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions { WriteIndented = true };

    private readonly IEventsStore _store;

    private readonly IList<PortEvent> _events = [];
    private readonly HashSet<Ship> _ships = [];
    private readonly HashSet<Port> _ports = [];

    public HarbourMaster(IEventsStore store)
    {
        _store = store;
    }

    public async Task<IOption<Guid>> RegisterShipAsync(string shipName, float weightCapacity)
    {
        ArgumentException.ThrowIfNullOrEmpty(shipName);
        
        var ship = new Ship(shipName, weightCapacity);

        var shipStored = await _store.AddShipAsync(ship);
        if (!shipStored.IsNone()) 
            return new Some<Guid>(shipStored.Reduce());
        
        Console.WriteLine($"[WARN]: {ship} not added. Ship {shipName} is just present");
        return new None<Guid>();
    }

    public async Task<IOption<Guid>> RegisterPortAsync(string portName)
    {
        ArgumentException.ThrowIfNullOrEmpty(portName);
        
        var port = new Port(portName);

        var portStored = await _store.AddPortAsync(port);
        if (!portStored.IsNone()) 
            return new Some<Guid>(portStored.Reduce());
        
        Console.WriteLine($"[WARN] Harbour Master: {port} not added. Port {portName} is just present");
        return new None<Guid>();
    }
    
    /// <summary>
    /// Load goods on a ship,
    /// let ship leaves the port for navigation to destination
    /// </summary>
    /// <param name="portId">Destination port id</param>
    /// <param name="shipId">Ship id</param>
    public async ValueTask SailAsync(Guid portId, Guid shipId)
    {
        var @event = new ShipHasSailed(DateTime.UtcNow, shipId, portId);
        await ApplyAsync(@event);
    }

    /// <summary>
    /// Host a ship in a port, unload goods and let it wait for the next trip
    /// </summary>
    /// <param name="portId">Arrival port id</param>
    /// <param name="shipId">Ship id</param>
    public async ValueTask DockAsync(Guid portId, Guid shipId)
    {
        var @event = new ShipHasDocked(DateTime.UtcNow, shipId, portId);
        await ApplyAsync(@event);
    }
    
    /// <summary>
    /// Locate a ship
    /// </summary>
    /// <param name="shipId">Ship id</param>
    public async Task LocateAsync(Guid shipId)
    {
        var ship = await GetShipAsync(shipId);
        
        var serialized = JsonSerializer.Serialize(new
        {
            Ship = ship
        }, _jsonSerializerOptions);
        
        Console.WriteLine("[DATA]:");
        Console.WriteLine(serialized);
    }

    /// <summary>
    /// Save current state to database
    /// </summary>
    public async Task SaveCurrentStateAsync()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public async Task HydrateAsync()
    {
        
    }

    private async ValueTask ApplyAsync(PortEvent @event)
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
        
        _events.Add(@event);
    }
    
    private async ValueTask<Ship> GetShipAsync(Guid id)
    {
        var s = _ships.SingleOrDefault(x => x.Id == id);
        if (s is not null)
            return s;
        
        var ship = await _store.GetShipAsync(id);
        if (ship.IsNone()) throw new ApplicationException($"Ship with id {id} not found");
        s = ship.Reduce();
        _ships.Add(s);

        return s;
    }
    
    private async ValueTask<Port> GetPortAsync(Guid id)
    {
        var p = _ports.SingleOrDefault(x => x.Id == id);
        if (p is not null) 
            return p;
        
        var port = await _store.GetPortAsync(id);
        if (port.IsNone()) throw new ApplicationException($"Port with id {id} not found");
        p = port.Reduce();
        _ports.Add(p);

        return p;
    }
}