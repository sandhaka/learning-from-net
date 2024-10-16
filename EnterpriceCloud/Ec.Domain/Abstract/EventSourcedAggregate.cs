using Ec.Domain.Dto;

// ReSharper disable ConvertToPrimaryConstructor

namespace Ec.Domain.Abstract;

public abstract class EventSourcedAggregate : IAggregateRoot
{
    protected readonly List<ISourceEvent> Changes = [];
    
    public StrongTypedId Id { get; protected set; }
    public int Version { get; protected set; }
    public int InitialVersion { get; protected set; }
    public IReadOnlyList<ISourceEvent> ChangesHistory => Changes.AsReadOnly();
    
    public void CausesEvent(ISourceEvent @event)
    {
        if (ApplyChange(@event).Success)
            Changes.Add(@event);
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

    protected abstract Feedback When(ISourceEvent @event, params object[] args);
    
    private Feedback ApplyChange(ISourceEvent @event)
    {
       var feedback = When(@event);
       if (feedback.Success) 
           Version++;
       
       return feedback;
    }
}