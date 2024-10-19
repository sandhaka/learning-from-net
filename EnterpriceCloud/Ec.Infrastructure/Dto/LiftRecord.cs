using Ec.Domain.Models;

namespace Ec.Infrastructure.Dto;

public sealed record LiftRecord(
    Guid Id,
    Guid BuildingId,
    double Longitude,
    double Latitude,
    int Floor,
    string Code)
{
    public static implicit operator Lift(LiftRecord record) =>
        new()
        {
            LiftId = new LiftId(record.Id),
            Code = record.Code,
            Location = Location.Create(new BuildingId(record.BuildingId), record.Longitude, record.Latitude,
                record.Floor)
        };

    public static implicit operator LiftRecord(Lift lift) =>
        new(lift.LiftId.Value, lift.Location.BuildingId.Value, lift.Location.Longitude,
            lift.Location.Latitude, lift.Location.Floor, lift.Code);
}