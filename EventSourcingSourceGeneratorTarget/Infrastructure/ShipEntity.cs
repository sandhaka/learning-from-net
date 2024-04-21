using EventSourcingSourceGeneratorTarget.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EventSourcingSourceGeneratorTarget.Infrastructure;

public sealed class ShipEntity
{
    public static implicit operator ShipEntity(Ship ship) =>
        new ShipEntity
        {
            Id = ship.Id,
            Name = ship.Name,
            WeightCapacity = ship.WeightCapacity
        };

    public static implicit operator Ship(ShipEntity entity) =>
        new Ship(entity.Id, entity.Name, entity.WeightCapacity);
    
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public float WeightCapacity { get; set; }
}