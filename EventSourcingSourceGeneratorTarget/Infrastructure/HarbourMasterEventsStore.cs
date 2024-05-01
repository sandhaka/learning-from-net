using EventSourcingSourceGeneratorTarget.Option;
using MongoDB.Driver;

namespace EventSourcingSourceGeneratorTarget.Infrastructure;

// TODO Auto-Generated
internal partial interface IEventsStore
{
    public Task<IEnumerable<PortEventData>> LoadAsync();
    public Task<Option<ShipEntity>> GetShipAsync(Guid shipId);
    public Task<Option<PortEntity>> GetPortAsync(Guid portId);
    public Task<(bool Added, Guid Id)> AddShipAsync(ShipEntity entity);
    public Task<(bool Added, Guid Id)> AddPortAsync(PortEntity entity);
    public Task SaveAsync(IReadOnlyList<PortEventData> events);
}

// TODO Auto-Generated
internal sealed partial class HarbourMasterEventsStore : IEventsStore
{
    private readonly IMongoDatabase _database;

    public HarbourMasterEventsStore()
    {
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING") ??
                               throw new ArgumentNullException("MONGODB_CONNECTIONSTRING");
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("es_source");
    }

    public async Task<IEnumerable<PortEventData>> LoadAsync()
    {
        var asyncCursor = await PortEventCollection().FindAsync(Builders<PortEventData>.Filter.Empty);
        return asyncCursor.ToEnumerable();
    }

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

    public async Task SaveAsync(IReadOnlyList<PortEventData> events)
    {
        var sortDefinition = Builders<PortEventData>.Sort.Descending(x => x.UtcDateTime);
        var filterDefinition = Builders<PortEventData>.Filter.Empty;

        var lastEvent = await PortEventCollection()
            .Find(filterDefinition)
            .Sort(sortDefinition)
            .Limit(1)
            .FirstOrDefaultAsync();
        
        var evts = events.Where(x => x.UtcDateTime > (lastEvent?.UtcDateTime ?? DateTime.MinValue));

        await PortEventCollection().InsertManyAsync(evts);
    }

    private IMongoCollection<PortEventData> PortEventCollection() =>
        _database.GetCollection<PortEventData>("portEvents");
    
    private IMongoCollection<PortEntity> PortCollection() =>
        _database.GetCollection<PortEntity>("ports");
    
    private IMongoCollection<ShipEntity> ShipCollection() =>
        _database.GetCollection<ShipEntity>("ships");
}