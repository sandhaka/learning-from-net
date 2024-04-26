using System.Text.Json;
using EventSourcingSourceGeneratorTarget.Infrastructure;

namespace EventSourcingSourceGeneratorTarget.Models;

/// <summary>
/// Simplified example model of a harbour master office, it holds the ships fleet and a ports collection.
/// With Dock() and Sail() methods track navigation activities.
/// </summary>
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

    /// <summary>
    /// Register a new ship
    /// </summary>
    /// <param name="shipName">Ship unique name</param>
    /// <param name="weightCapacity">Cargo Capacity in Kg</param>
    /// <returns>Unique identifier of the ship entity</returns>
    public async Task<Guid> RegisterShipAsync(string shipName, float weightCapacity)
    {
        ArgumentException.ThrowIfNullOrEmpty(shipName);
        
        var ship = new Ship(shipName, weightCapacity);

        var shipStored = await _store.AddShipAsync(ship);
        
        if (!shipStored.Added)
            Console.WriteLine($"[WARN] Harbour Master: Ship not added. Ship {shipName} is just present");
        
        return shipStored.Id;
    }

    /// <summary>
    /// Register a new port
    /// </summary>
    /// <param name="portName">Port unique name</param>
    /// <returns>Unique identifier of the port entity</returns>
    public async Task<Guid> RegisterPortAsync(string portName)
    {
        ArgumentException.ThrowIfNullOrEmpty(portName);
        
        var port = new Port(portName);

        var portStored = await _store.AddPortAsync(port);
        
        if (!portStored.Added)
            Console.WriteLine($"[WARN] Harbour Master: Port not added. Port {portName} is just present");
        
        return portStored.Id;
    }
    
    /// <summary>
    /// Load goods on a ship,
    /// let ship leaves the port for navigation to destination
    /// </summary>
    /// <param name="portId">Destination port id</param>
    /// <param name="shipId">Ship id</param>
    public async ValueTask SailAsync(Guid portId, Guid shipId)
    {
        ArgumentNullException.ThrowIfNull(portId, nameof(portId));
        ArgumentNullException.ThrowIfNull(shipId, nameof(shipId));
        
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
        ArgumentNullException.ThrowIfNull(portId, nameof(portId));
        ArgumentNullException.ThrowIfNull(shipId, nameof(shipId));
        
        var @event = new ShipHasDocked(DateTime.UtcNow, shipId, portId);
        await ApplyAsync(@event);
    }
    
    /// <summary>
    /// Locate a ship
    /// </summary>
    /// <param name="shipId">Ship id</param>
    public async Task LocateAsync(Guid shipId)
    {
        ArgumentNullException.ThrowIfNull(shipId, nameof(shipId));
        
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
        var eventsToStore = _events
            .Select(e => (PortEventData) e)
            .ToList()
            .AsReadOnly();
        
        await _store.SaveAsync(eventsToStore);
    }
    
    /// <summary>
    /// Restore previous state from the database
    /// </summary>
    public async Task HydrateAsync()
    {
        if (_events.Any())
            throw new ApplicationException($"Object {GetType().Name} is initialized. Create a new instance and retry.");
            
        var events = await _store.GetPortEventsAsync();

        foreach (var @event in events)
        {
            var eventDomainObject = (PortEvent) @event;
            await ApplyAsync(eventDomainObject);
            _events.Add(eventDomainObject);
        }
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