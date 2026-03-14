using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.ValueObjects;

public sealed class GpsCoordinates : IEquatable<GpsCoordinates>
{
    public double Latitude { get; }
    public double Longitude { get; }

    private GpsCoordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GpsCoordinates Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new DomainException("GPS_LATITUDE_INVALID", "Latitude must be between -90 and 90.");
        if (longitude is < -180 or > 180)
            throw new DomainException("GPS_LONGITUDE_INVALID", "Longitude must be between -180 and 180.");

        return new GpsCoordinates(latitude, longitude);
    }

    public bool Equals(GpsCoordinates? other) =>
        other is not null &&
        Math.Abs(Latitude - other.Latitude) < 0.0001 &&
        Math.Abs(Longitude - other.Longitude) < 0.0001;
    public override bool Equals(object? obj) => obj is GpsCoordinates g && Equals(g);
    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    public override string ToString() => $"{Latitude:F6},{Longitude:F6}";
}
