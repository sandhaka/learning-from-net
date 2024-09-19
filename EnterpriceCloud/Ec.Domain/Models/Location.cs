namespace Ec.Domain.Models;

public sealed record Location
{
    public required (double Longitute, double Latitude) Coordinates { get; init; }
    public required int Floor { get; init; }

    public static Location None => new() { Coordinates = (0, 0), Floor = 0 };
}