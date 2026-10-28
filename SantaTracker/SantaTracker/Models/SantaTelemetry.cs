namespace SantaTracker.Models;

/// <summary>
/// Real-time telemetry data from Santa's sleigh
/// </summary>
public partial record SantaTelemetry(
    long ToysDelivered,
    long CookiesEaten,
    double DistanceTraveled,
    GeoCoordinate CurrentLocation,
    string CurrentCity,
    string CurrentCountry,
    DateTimeOffset LastUpdate);

/// <summary>
/// Geographic coordinate
/// </summary>
public partial record GeoCoordinate(double Latitude, double Longitude);
