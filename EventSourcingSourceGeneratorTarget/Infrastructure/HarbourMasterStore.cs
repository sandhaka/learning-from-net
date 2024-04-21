using EventSourcingSourceGeneratorTarget.Option;
using MongoDB.Driver;

namespace EventSourcingSourceGeneratorTarget.Infrastructure;

// TODO Auto-Generated
internal partial interface IEventsStore
{
    public Task<Option<ShipEntity>> GetShipAsync(Guid shipId);
    public Task<Option<PortEntity>> GetPortAsync(Guid portId);
    public Task<Option<Guid>> AddShipAsync(ShipEntity entity);
    public Task<Option<Guid>> AddPortAsync(PortEntity entity);
}

// TODO Auto-Generated
internal sealed partial class HarbourMasterStore : IEventsStore
{
    private readonly IMongoDatabase _database;

    public HarbourMasterStore()
    {
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING") ??
                               throw new ArgumentNullException("MONGODB_CONNECTIONSTRING");
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("es_source");
    }

    public async Task<Option<ShipEntity>> GetShipAsync(Guid shipId)
    {
        var shipSearchFilter = Builders<ShipEntity>.Filter.Eq(x => x.Id, shipId);
        var shipSearch = await ShipCollection().FindAsync(shipSearchFilter);

        if (!await shipSearch.AnyAsync())
            return new None<ShipEntity>();

        var entity = await shipSearch.SingleAsync();
        
        return new Some<ShipEntity>(entity);
    }
    
    public async Task<Option<PortEntity>> GetPortAsync(Guid portId)
    {
        var portSearchFilter = Builders<PortEntity>.Filter.Eq(x => x.Id, portId);
        var portSearch = await PortCollection().FindAsync(portSearchFilter);

        if (!await portSearch.AnyAsync())
            return new None<PortEntity>();

        var entity = await portSearch.SingleAsync();
        
        return new Some<PortEntity>(entity);
    }

    public async Task<Option<Guid>> AddShipAsync(ShipEntity entity)
    {
        var shipSearchFilter = Builders<ShipEntity>.Filter.Eq(x => x.Name, entity.Name);
        
        var shipSearch = await ShipCollection().FindAsync(shipSearchFilter);

        if (await shipSearch.AnyAsync())
            return new None<Guid>();

        await ShipCollection().InsertOneAsync(entity);

        return entity.Id;
    }
    
    public async Task<Option<Guid>> AddPortAsync(PortEntity entity)
    {
        var portSearchFilter = Builders<PortEntity>.Filter.Eq(x => x.Name, entity.Name);
        
        var portSearch = await PortCollection().FindAsync(portSearchFilter);

        if (await portSearch.AnyAsync())
            return new None<Guid>();

        await PortCollection().InsertOneAsync(entity);

        return entity.Id;
    }

    public async Task SaveAsync(IReadOnlyList<PortEventData> events)
    {
        
    }

    private IMongoCollection<PortEventData> PortEventCollection() =>
        _database.GetCollection<PortEventData>("portEvents");
    
    private IMongoCollection<PortEntity> PortCollection() =>
        _database.GetCollection<PortEntity>("ports");
    
    private IMongoCollection<ShipEntity> ShipCollection() =>
        _database.GetCollection<ShipEntity>("ships");
}