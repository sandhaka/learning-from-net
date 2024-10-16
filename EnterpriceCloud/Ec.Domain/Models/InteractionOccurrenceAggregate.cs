using System.Diagnostics.CodeAnalysis;
using System.Text;
using Ec.Domain.Abstract;
using Ec.Domain.Dto;

namespace Ec.Domain.Models;

public class InteractionOccurrenceAggregate : EventSourcedAggregate
{
    public required UserId UserId { get; init; }
    public Location CurrentLocation { get; private set; } = Location.NoLocation;
    public string LatestInteractionSubject { get; private set; } = string.Empty;
    public StringBuilder InteractionPath { get; } = new();
    public string Path => InteractionPath.ToString();
    
    [SetsRequiredMembers]
    private InteractionOccurrenceAggregate(UserId userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        UserId = userId;
    }
    
    protected override Feedback When(ISourceEvent @event, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(args[0]);

        var element = args[0] as IBuildingElement ?? throw new ArgumentNullException(nameof(args)); 
        var elementLocation = element.Location;

        if (CurrentLocation.Equals(Location.NoLocation))
            CurrentLocation = Location.Initial(element.Location.BuildingId);
        
        CurrentLocation.SetCoordinates(
            elementLocation.Longitude, 
            elementLocation.Latitude);

        LatestInteractionSubject = element.GetType().Name;
        
        switch (@event)
        {
            case Interacted:
            {
                InteractionPath.Append($" > At [{element.Description}](${CurrentLocation}) ");
                
                // Lift interaction moving to another floor
                if (element is Lift liftRide)
                {
                    CurrentLocation.SetFloor(liftRide.DestinationFloor);
                }
                
                break;
            }
            case Leaved:
            {
                InteractionPath.Append($" > (${CurrentLocation}) --- ");
                
                break;
            }
            default: return Feedback.Failure($"Unknown event type: {@event.GetType().Name}");
        }
        
        return Feedback.Successful();
    }
}