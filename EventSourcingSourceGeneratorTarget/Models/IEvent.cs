namespace EventSourcingSourceGeneratorTarget.Models;

/// <summary>
/// Characterize an event
/// </summary>
internal interface IEvent
{
    public Guid Id { get; }
    public DateTime UtcDateTime { get; }
}