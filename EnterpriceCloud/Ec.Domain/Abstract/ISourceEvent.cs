using Ec.Domain.Models;

namespace Ec.Domain.Abstract;

public interface ISourceEvent
{
    Guid EventId { get; }
    UserId UserId { get; }
    DateTime Timestamp { get; }
    Guid BuildingElementId { get; }
}