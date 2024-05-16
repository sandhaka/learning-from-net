using EventSourcingSourceGeneratorTarget.Option;
using MongoDB.Driver;

namespace EventSourcingSourceGeneratorTarget.Infrastructure;

internal partial interface IHarbourMasterEventsStore
{
    public Task<Option<ShipEntity>> GetShipAsync(Guid shipId);
    public Task<Option<PortEntity>> GetPortAsync(Guid portId);
    public Task<(bool Added, Guid Id)> AddShipAsync(ShipEntity entity);
    public Task<(bool Added, Guid Id)> AddPortAsync(PortEntity entity);
}

/// <summary>
/// Additional methods for events store
/// </summary>
internal sealed partial class HarbourMasterEventsStore : IHarbourMasterEventsStore
{
    public async Task<Option<ShipEntity>> GetShipAsync(Guid shipId)
    {
        var filterDefinition = Builders<ShipEntity>.Filter.Eq(x => x.Id, shipId);
        var asyncCursor = await ShipCollection().FindAsync(filterDefinition);

        var found = await asyncCursor.ToListAsync();
        
        if (found.Count == 0)
            return new None<ShipEntity>();
            
        var entity = found.Single();
        
        return new Some<ShipEntity>(entity);
    }
    
    public async Task<Option<PortEntity>> GetPortAsync(Guid portId)
    {
        var portSearchFilter = Builders<PortEntity>.Filter.Eq(x => x.Id, portId);
        var asyncCursor = await PortCollection().FindAsync(portSearchFilter);

        var found = await asyncCursor.ToListAsync();

        if (found.Count == 0)
            return new None<PortEntity>();

        var entity = found.Single();
        
        return new Some<PortEntity>(entity);
    }

    public async Task<(bool Added, Guid Id)> AddShipAsync(ShipEntity entity)
    {
        var filterDefinition = Builders<ShipEntity>.Filter.Eq(x => x.Name, entity.Name);
        
        var asyncCursor = await ShipCollection().FindAsync(filterDefinition);

        var found = await asyncCursor.ToListAsync();
        
        if (found.Count != 0)
        {
            var ship = found.Single();
            return (Added: false, ship.Id);
        }

        await ShipCollection().InsertOneAsync(entity);

        return (Added: true, entity.Id);
    }
    
    public async Task<(bool Added, Guid Id)> AddPortAsync(PortEntity entity)
    {
        var filterDefinition = Builders<PortEntity>.Filter.Eq(x => x.Name, entity.Name);
        
        var asyncCursor = await PortCollection().FindAsync(filterDefinition);

        var found = await asyncCursor.ToListAsync();
        
        if (found.Count != 0)
        {
            var port = found.Single();
            return (Added: false, port.Id);
        }

        await PortCollection().InsertOneAsync(entity);

        return (Added: true, entity.Id);
    }
    
    private IMongoCollection<PortEntity> PortCollection() =>
        _database.GetCollection<PortEntity>("ports");
    
    private IMongoCollection<ShipEntity> ShipCollection() =>
        _database.GetCollection<ShipEntity>("ships");
}