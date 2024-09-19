using System.Diagnostics.CodeAnalysis;
using Ec.Domain.Abstract;
// ReSharper disable ConvertToPrimaryConstructor

namespace Ec.Domain.Models.Abstract;

public abstract class EventSourcedAggregate : IAggregateRoot
{
    private readonly List<ISourceEvent> _changes = [];
    
    public StrongTypedId Id { get; protected set; }
    public int Version { get; protected set; }
    public int InitialVersion { get; protected set; }
    public IReadOnlyList<ISourceEvent> Changes => _changes.AsReadOnly();
    
    public void CausesEvent(ISourceEvent @event)
    {
        _changes.Add(@event);
        ApplyChange(@event);
    }
    
    protected EventSourcedAggregate()
    {
        Version = 0;
        InitialVersion = 0;
    }
    
    protected EventSourcedAggregate(StrongTypedId id)
    {
        Id = id;
        Version = 0;
        InitialVersion = 0;
    }

    protected EventSourcedAggregate(StrongTypedId id, int version, IEnumerable<ISourceEvent> changes)
    {
        Id = id;
        Version = version;
        InitialVersion = version;
        
        foreach (var @event in changes)
            CausesEvent(@event);
    }

    protected abstract void When(ISourceEvent @event);
    
    private void ApplyChange(ISourceEvent @event)
    {
       When(@event);
       Version++;
    }
}