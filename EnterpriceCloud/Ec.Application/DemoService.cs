using EasyNetQ;
using Ec.Domain.Abstract;
using Ec.Domain.Models;

namespace Ec.Application;

public sealed class DemoService
{
    private readonly IBus _bus;

    // The user / person data
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _physicalPersonId = Guid.NewGuid();

    private readonly User _demoUser;
    

    private readonly Guid _buildingElementId = Guid.NewGuid();
    // Building entrance
    private readonly Guid _mainEntranceId = Guid.NewGuid();
    // Office entrances
    private readonly Guid _officeEntranceAtFloor1Id = Guid.NewGuid();
    private readonly Guid _officeEntranceAtFloor2Id = Guid.NewGuid();
    private readonly Guid _officeEntranceAtFloor3Id = Guid.NewGuid();
    // Elevators entrances
    private readonly Guid _liftAtFloor0Id = Guid.NewGuid();
    private readonly Guid _liftAtFloor1Id = Guid.NewGuid();
    private readonly Guid _liftAtFloor2Id = Guid.NewGuid();
    private readonly Guid _liftAtFloor3Id = Guid.NewGuid();
    // Desktop at 3rd floor's office
    private readonly Guid _desktopAtOfficeAtFloor3Id = Guid.NewGuid();

    private readonly Building _building;
    private readonly Entry _mainEntrance;
    private readonly Entry _officeEntranceAtFloor1;
    private readonly Entry _officeEntranceAtFloor2;
    private readonly Entry _officeEntranceAtFloor3;
    private readonly Lift _liftAtFloor0;
    private readonly Lift _liftAtFloor1;
    private readonly Lift _liftAtFloor2;
    private readonly Lift _liftAtFloor3;
    private readonly Desktop _desktopAtOfficeAtFloor3;

    public DemoService(IBus bus)
    {
        _bus = bus; 
        
        _demoUser = new User
        {
            Email = "demo.user@sampledomain.email",
            UserId = new UserId(_userId),
            PhysicalPersonId = new PhysicalPersonId((_physicalPersonId))
        };

        _building = new Building
        {
            BuildingId = new BuildingId(_buildingElementId),
            Address = "Demo building address",
            Name = "Demo building",
            City = "Demo city",
            Country = "Demo country",
            Description = "Demo building description",
            Floors = 3,
            LowerFloor = 0,
            State = "Demo state",
            Zip = "12345"
        };

        // Building main entrance
        _mainEntrance = new Entry
        {
            EntryId = new EntryId(_mainEntranceId),
            Location = Location.Create(_building.BuildingId, 0, 0, 0),
            Code = "mainEntrance",
        };

        // Building elevator 
        _liftAtFloor0 = new Lift
        {
            LiftId = new LiftId(_liftAtFloor0Id),
            Location = Location.Create(_building.BuildingId, 2, 2, 0),
            Code = "liftAtFloor0"
        };
        _liftAtFloor1 = new Lift
        {
            LiftId = new LiftId(_liftAtFloor1Id),
            Location = Location.Create(_building.BuildingId, 2, 2, 1),
            Code = "liftAtFloor1"
        };
        _liftAtFloor2 = new Lift
        {
            LiftId = new LiftId(_liftAtFloor2Id),
            Location = Location.Create(_building.BuildingId, 2, 2, 2),
            Code = "liftAtFloor2"
        };
        _liftAtFloor3 = new Lift
        {
            LiftId = new LiftId(_liftAtFloor3Id),
            Location = Location.Create(_building.BuildingId, 2, 2, 3),
            Code = "liftAtFloor3"
        };
        
        // Office entrances
        var officeEntranceAtFloor1 = new Entry
        {
            EntryId = new EntryId(_officeEntranceAtFloor1Id),
            Location = Location.Create(_building.BuildingId, 1, 1, 1),
            Code = "officeEntranceAtFloor1"
        };
        var officeEntranceAtFloor2 = new Entry
        {
            EntryId = new EntryId(_officeEntranceAtFloor2Id),
            Location = Location.Create(_building.BuildingId, 1, 1, 2),
            Code = "officeEntranceAtFloor2"
        };
        var officeEntranceAtFloor3 = new Entry
        {
            EntryId = new EntryId(_officeEntranceAtFloor3Id),
            Location = Location.Create(_building.BuildingId, 1, 1, 3),
            Code = "officeEntranceAtFloor3"
        };
    }

    public async Task RunDemoAsync(CancellationToken cancellationToken = default)
    {
        // Demo sequence of user interaction events:
        IEnumerable<ISourceEvent> events =
        [
            new Interacted
            {
                InteractedId = new InteractedId(Guid.NewGuid()),
                UserId = _demoUser.UserId,
                Timestamp = DateTime.UtcNow,
                BuildingElementId = _building.BuildingId.Value,
            }
            
        ];

        foreach (var @event in events)
        {
            await _bus.PubSub.PublishAsync(@event, cancellationToken);
            
            if (cancellationToken.IsCancellationRequested)
                break;
            
            Thread.Sleep(1000);
        }
    }
}