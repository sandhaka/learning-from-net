using EventSourcingSourceGeneratorTarget.Infrastructure;
using EventSourcingSourceGeneratorTarget.Models;

var firstHarbourMoInstance = new HarbourMaster(new HarbourMasterStore());

#region  [Sample setup]
var stMaria = await firstHarbourMoInstance.RegisterShipAsync("St.Maria", 8000.00f);
var mayFlower = await firstHarbourMoInstance.RegisterShipAsync("MayFlower", 3000.00f);
var africanQueen = await firstHarbourMoInstance.RegisterShipAsync("AfricanQueen", 17000.00f);
var belfastHarbour = await firstHarbourMoInstance.RegisterPortAsync("BelfastHarbour");
var alabamaPort = await firstHarbourMoInstance.RegisterPortAsync("AlabamaPort");
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

await firstHarbourMoInstance.SailAsync(belfastHarbourPortId, stMariaShipId);
await firstHarbourMoInstance.SailAsync(belfastHarbourPortId, mayFlowerId);
Thread.Sleep(100);
await firstHarbourMoInstance.DockAsync(alabamaPortId, stMariaShipId);
// ... Various navigation activities
Thread.Sleep(300);
await firstHarbourMoInstance.SailAsync(alabamaPortId, stMariaShipId);
Thread.Sleep(200);
await firstHarbourMoInstance.DockAsync(belfastHarbourPortId, stMariaShipId);
await firstHarbourMoInstance.DockAsync(alabamaPortId, mayFlowerId);

// Locate a ship:
await firstHarbourMoInstance.LocateAsync(stMariaShipId);

// Saving current state to a persistence layer
await firstHarbourMoInstance.SaveCurrentStateAsync();
// ...

var anotherHarbourMoInstance = new HarbourMaster(new HarbourMasterStore());

// Reload from saved data
await anotherHarbourMoInstance.HydrateAsync();

// Locate a ship:
await anotherHarbourMoInstance.LocateAsync(stMariaShipId);

await anotherHarbourMoInstance.LocateAsync(mayFlowerId);
await anotherHarbourMoInstance.LocateAsync(africanQueenId);


return 0;

#endregion