using Ec.Domain.Models;

namespace Ec.Infrastructure.Dto;

public sealed record BuildingRecord(
    Guid Id,
    int Floors,
    string Name,
    string Description,
    string Address,
    string City,
    string State,
    string Zip,
    string Country,
    int LowerFloor)
{
    public static implicit operator Building(BuildingRecord record) =>
        new()
        {
            BuildingId = new BuildingId(record.Id),
            Floors = record.Floors,
            Name = record.Name,
            Description = record.Description,
            Address = record.Address,
            City = record.City,
            State = record.State,
            Zip = record.Zip,
            Country = record.Country,
            LowerFloor = record.LowerFloor
        };

    public static implicit operator BuildingRecord(Building building) =>
        new(building.BuildingId.Value, building.Floors, building.Name, building.Description,
            building.Address, building.City, building.State, building.Zip, building.Country, building.LowerFloor);
}