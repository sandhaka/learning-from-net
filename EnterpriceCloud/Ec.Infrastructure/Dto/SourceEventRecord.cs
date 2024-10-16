using Ec.Domain.Abstract;
using Ec.Domain.Models;

namespace Ec.Infrastructure.Dto;

public record SourceEventRecord(
    Guid EventId,
    Guid UserId,
    DateTime Timestamp,
    string EventTypeAssemblyQualifiedName,
    Guid BuildingElementId)
{
    public static ISourceEvent ToSourceEvent(SourceEventRecord record) =>
        record.EventTypeAssemblyQualifiedName switch
        {
            "Ec.Domain.Models.Leaved" => new Leaved
            {
                LeavedId = new LeavedId(record.EventId),
                UserId = new UserId(record.UserId),
                Timestamp = record.Timestamp,
                BuildingElementId = record.BuildingElementId
            },
            "Ec.Domain.Models.Interacted" => new Interacted
            {
                InteractedId = new InteractedId(record.EventId),
                UserId = new UserId(record.UserId),
                Timestamp = record.Timestamp,
                BuildingElementId = record.BuildingElementId
            },
            _ => throw new NotImplementedException(nameof(record.EventTypeAssemblyQualifiedName))
        };

    public static SourceEventRecord FromSourceEvent(ISourceEvent sourceEvent) =>
        new(sourceEvent.EventId, sourceEvent.UserId.Value, sourceEvent.Timestamp,
            sourceEvent.GetType().AssemblyQualifiedName!, sourceEvent.BuildingElementId);
}