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
    public bool UsingElevator { get; private set; } = false;
    
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
                InteractIfTheElementIsALift(element);
                // TODO
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
    
    private bool InteractIfTheElementIsALift(IBuildingElement element)
    {
        // Lift interaction moving to another floor
        if (element is Lift lift)
        {
            if (UsingElevator)
            {
                UsingElevator = false;
                // Out from elevator to another floor
                CurrentLocation.SetFloor(lift.Location.Floor);
            }
            else
            {
                // Otherwise start using the elevator
                UsingElevator = true;
            }

            return true;
        }

        // User can exit from elevator without moving to another floor,
        // user can change his mind... 
        UsingElevator = false;
        
        return UsingElevator;
    }
}