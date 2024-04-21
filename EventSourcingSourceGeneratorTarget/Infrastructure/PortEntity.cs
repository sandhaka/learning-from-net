using EventSourcingSourceGeneratorTarget.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EventSourcingSourceGeneratorTarget.Infrastructure;

public sealed class PortEntity
{
    public static implicit operator PortEntity(Port port) =>
        new PortEntity
        {
            Id = port.Id,
            Name = port.Name,
        };

    public static implicit operator Port(PortEntity entity) =>
        new Port(entity.Id, entity.Name);

    [BsonId(IdGenerator = typeof(GuidGenerator))]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}