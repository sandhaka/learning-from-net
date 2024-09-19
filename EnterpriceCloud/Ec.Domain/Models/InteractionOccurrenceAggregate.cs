using System.Diagnostics.CodeAnalysis;
using Ec.Domain.Abstract;
using Ec.Domain.Models.Abstract;

namespace Ec.Domain.Models;

public class InteractionOccurrenceAggregate : EventSourcedAggregate
{
    private DateTime _occurrenceStart = DateTime.MinValue;
    public required UserId UserId { get; init; }
    public Location CurrentLocation { get; private set; } = Location.None;

    [SetsRequiredMembers]
    private InteractionOccurrenceAggregate(UserId userId)
    {
        UserId = userId;
    }
    
    [SetsRequiredMembers]
    private InteractionOccurrenceAggregate(StrongTypedId id, UserId userId) : base(id)
    {
        UserId = userId;
    }

    [SetsRequiredMembers]
    private InteractionOccurrenceAggregate(
        StrongTypedId id, 
        int version, 
        IEnumerable<ISourceEvent> changes,
        UserId userId) : base(id, version, changes)
    {
        UserId = userId;
    }

    public static InteractionOccurrenceAggregate New(UserId userId) => new (userId);

    protected override void When(ISourceEvent @event)
    {
        throw new NotImplementedException();
    }
}