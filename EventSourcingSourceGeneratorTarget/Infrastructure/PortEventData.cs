using EventSourcingSourceGeneratorTarget.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EventSourcingSourceGeneratorTarget.Infrastructure;

public sealed class PortEventData
{
    public static implicit operator PortEventData(PortEvent portEvent) =>
        new PortEventData
        {
            Id = portEvent.Id,
            UtcDateTime = portEvent.UtcDateTime,
            ShipId = portEvent.ShipId,
            PortId = portEvent.PortId,
            TypeName = portEvent.GetType().Name
        };
        
    public static implicit operator PortEvent(PortEventData data) =>
        data.TypeName switch
        {
            "ShipHasSailed" => new ShipHasSailed(data.UtcDateTime, data.ShipId, data.PortId) { Id = data.Id },
            "ShipHasDocked" => new ShipHasDocked(data.UtcDateTime, data.ShipId, data.PortId) { Id = data.Id },
            _ => throw new ArgumentOutOfRangeException(data.TypeName)
        };
    
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    public Guid Id { get; set; }
    public DateTime UtcDateTime { get; set; }
    public Guid ShipId { get; set; }
    public Guid PortId { get; set; }
    public string TypeName { get; set; } = null!;
}