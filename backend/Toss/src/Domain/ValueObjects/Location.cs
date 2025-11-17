namespace Toss.Domain.ValueObjects;

public class Location : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Area { get; private set; }
    public string? Zone { get; private set; }

    private Location() { }

    public Location(double latitude, double longitude, string? area = null, string? zone = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180");

        Latitude = latitude;
        Longitude = longitude;
        Area = area;
        Zone = zone;
    }

    public double DistanceInKilometersTo(Location other)
    {
        // Haversine formula
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var lat1 = DegreesToRadians(Latitude);
        var lat2 = DegreesToRadians(other.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public override string ToString()
    {
        var parts = new List<string> { $"{Latitude:F6}, {Longitude:F6}" };
        if (!string.IsNullOrWhiteSpace(Zone)) parts.Add(Zone);
        if (!string.IsNullOrWhiteSpace(Area)) parts.Add(Area);
        return string.Join(" - ", parts);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
        yield return Area ?? string.Empty;
        yield return Zone ?? string.Empty;
    }
}

