using EventSourcingSourceGeneratorTarget.Infrastructure;
using EventSourcingSourceGeneratorTarget.Models;

var firstHarbourMoInstance = new HarbourMaster(new HarbourMasterEventsStore());

#region  [Sample setup]
var stMariaShipId = await firstHarbourMoInstance.RegisterShipAsync("St.Maria", 8000.00f);
var mayFlowerId = await firstHarbourMoInstance.RegisterShipAsync("MayFlower", 3000.00f);
var africanQueenId = await firstHarbourMoInstance.RegisterShipAsync("AfricanQueen", 17000.00f);
var belfastHarbourPortId = await firstHarbourMoInstance.RegisterPortAsync("BelfastHarbour");
var alabamaPortId = await firstHarbourMoInstance.RegisterPortAsync("AlabamaPort");
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

var anotherHarbourMoInstance = new HarbourMaster(new HarbourMasterEventsStore());

// Reload from saved data
await anotherHarbourMoInstance.HydrateAsync();

// Locate a ship:
await anotherHarbourMoInstance.LocateAsync(stMariaShipId);

await anotherHarbourMoInstance.LocateAsync(mayFlowerId);
await anotherHarbourMoInstance.LocateAsync(africanQueenId);


return 0;

#endregion