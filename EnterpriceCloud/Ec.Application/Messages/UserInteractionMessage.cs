using Ec.Domain.Abstract;

namespace Ec.Application.Messages;

public sealed class UserInteractionMessage
{
    public readonly DateTime Timestamp = DateTime.UtcNow;
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public required ISourceEvent InteractionEvent { get; init; }
}