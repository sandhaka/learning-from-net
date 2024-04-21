using EventSourcingSourceGeneratorTarget.Infrastructure;
using EventSourcingSourceGeneratorTarget.Models;

var harbourMasterOffice = new HarbourMaster(new EventsStore());

#region  [Sample setup]
var stMaria = harbourMasterOffice.RegisterShip("St.Maria", 8000.00f);
var mayFlower = harbourMasterOffice.RegisterShip("MayFlower", 3000.00f);
var africanQueen = harbourMasterOffice.RegisterShip("AfricanQueen", 17000.00f);
var belfastHarbour = harbourMasterOffice.RegisterPort("BelfastHarbour");
var alabamaPort = harbourMasterOffice.RegisterPort("AlabamaPort");
if (new[] { stMaria, mayFlower, africanQueen }.Any(registered => registered.IsNone()))
{
    Console.WriteLine("[ERR]: Ship registration failed");
    return -1;
}
var stMariaShipId = stMaria.Reduce();
var mayFlowerId = mayFlower.Reduce();
var belfastHarbourPortId = belfastHarbour.Reduce();
var africanQueenId = africanQueen.Reduce();
var alabamaPortId = alabamaPort.Reduce();
#endregion

#region [Scenario]

/*
 * Suppose to have the following navigation events registered by the harbour
 * master office.
 */

harbourMasterOffice.Sail(belfastHarbourPortId, stMariaShipId);
harbourMasterOffice.Sail(belfastHarbourPortId, mayFlowerId);
Thread.Sleep(100);
harbourMasterOffice.Dock(alabamaPortId, stMariaShipId);
// ... Various navigation activities
Thread.Sleep(300);
harbourMasterOffice.Sail(alabamaPortId, stMariaShipId);
Thread.Sleep(200);
harbourMasterOffice.Dock(belfastHarbourPortId, stMariaShipId);
harbourMasterOffice.Dock(alabamaPortId, mayFlowerId);

// Locate a ship:
harbourMasterOffice.Locate(stMariaShipId);

// Saving current state to a persistence layer
harbourMasterOffice.Save();
// ...
// Reload from saved data
harbourMasterOffice.Load();

// Locate a ship:
harbourMasterOffice.Locate(stMariaShipId);

harbourMasterOffice.Locate(mayFlowerId);
harbourMasterOffice.Locate(africanQueenId);


return 0;

#endregion