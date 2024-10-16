namespace Ec.Domain.Models;

public sealed record Location
{
    public BuildingId BuildingId { get; private set; }
    public double Longitude { get; private set; } = 0;
    public double Latitude { get; private set; } = 0;
    public int Floor { get; private set; } = 0;

    /// <summary>
    /// Initializes a new Location instance with default coordinates and floor.
    /// </summary>
    /// <param name="buildingId">The identifier of the building.</param>
    /// <returns>A new Location instance with the specified BuildingId, default coordinates (0,0), and floor 0.</returns>
    public static Location Initial(BuildingId buildingId) => new() { BuildingId = buildingId };

    /// <summary>
    /// Creates a Location instance representing no specific location, i.e., with an empty BuildingId.
    /// </summary>
    /// <returns>A Location instance with an empty BuildingId and default coordinates (0,0) and floor 0.</returns>
    public static Location NoLocation => new() { BuildingId = new BuildingId(Guid.Empty) };

    /// <summary>
    /// Creates a new Location instance with specified buildingId, longitude, latitude, and floor.
    /// </summary>
    /// <param name="buildingId">The identifier of the building.</param>
    /// <param name="longitude">The longitude of the location.</param>
    /// <param name="latitude">The latitude of the location.</param>
    /// <param name="floor">The floor number of the location.</param>
    /// <returns>A new Location instance with specified buildingId, longitude, latitude, and floor.</returns>
    public static Location Create(BuildingId buildingId, double longitude, double latitude, int floor) =>
        new()
        {
            BuildingId = buildingId,
            Floor = floor,
            Longitude = longitude,
            Latitude = latitude
        };

    /// <summary>
    /// Sets the coordinates (longitude and latitude) for the Location instance.
    /// </summary>
    /// <param name="longitude">The longitude value to set. Must be non-negative.</param>
    /// <param name="latitude">The latitude value to set. Must be non-negative.</param>
    public void SetCoordinates(double longitude, double latitude)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(longitude);
        ArgumentOutOfRangeException.ThrowIfNegative(latitude);
        
        Longitude = longitude;
        Latitude = latitude;
    }
    
    public void SetFloor(int floor) => Floor = floor;
    
    public override string ToString() => $"(Floor: {Floor}), [{Longitude:F2}, {Latitude:F2}]";
}