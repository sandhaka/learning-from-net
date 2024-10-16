using Ec.Domain.Models;

namespace Ec.Infrastructure.Dto;

public sealed record EntryRecord(
    Guid Id, 
    Guid BuildingId, 
    double Longitude, 
    double Latitude, 
    int Floor, 
    string Code)
{
    public static implicit operator Entry(EntryRecord record) =>
        new()
        {
            EntryId = new EntryId(record.Id),
            Location = Location.Create(new BuildingId(record.BuildingId), record.Longitude, record.Latitude,
                record.Floor),
            Code = record.Code
        };

    public static implicit operator EntryRecord(Entry entry) =>
        new(entry.EntryId.Value, entry.Location.BuildingId.Value, entry.Location.Longitude,
            entry.Location.Latitude, entry.Location.Floor, entry.Code);
}