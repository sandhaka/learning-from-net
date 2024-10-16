using Ec.Domain.Models;

namespace Ec.Infrastructure.Dto;

public sealed record DesktopRecord(
    Guid Id, 
    Guid BuildingId, 
    double Longitude, 
    double Latitude, 
    int Floor, 
    string Code)
{
    public static implicit operator Desktop(DesktopRecord record) =>
        new()
        {
            DesktopId = new DesktopId(record.Id),
            Code = record.Code,
            Location = Location.Create(new BuildingId(record.BuildingId), record.Longitude, record.Latitude,
                record.Floor)
        };

    public static implicit operator DesktopRecord(Desktop desktop) =>
        new(desktop.DesktopId.Value, desktop.Location.BuildingId.Value, desktop.Location.Longitude,
            desktop.Location.Latitude, desktop.Location.Floor, desktop.Code);
}