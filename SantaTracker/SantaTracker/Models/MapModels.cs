using System.Text.Json.Serialization;

namespace SantaTracker.Models;

/// <summary>
/// Represents a geographic point (latitude/longitude)
/// </summary>
public record GeoPoint(double Latitude, double Longitude)
{
    /// <summary>
    /// Returns coordinates as [lon, lat] array for GeoJSON
    /// </summary>
    public double[] ToGeoJsonCoordinate() => [Longitude, Latitude];
}

/// <summary>
/// Represents Santa's current position and movement state
/// </summary>
public record SantaPosition(
    double Latitude,
    double Longitude,
    double Heading = 0,
    double Speed = 0,
    string? NextStop = null,
    string? Eta = null)
{
    public GeoPoint ToGeoPoint() => new(Latitude, Longitude);
}

/// <summary>
/// GeoJSON Feature for route data
/// </summary>
public class GeoJsonFeature
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Feature";

    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    [JsonPropertyName("geometry")]
    public GeoJsonGeometry Geometry { get; set; } = new();
}

/// <summary>
/// GeoJSON Geometry (LineString for routes)
/// </summary>
public class GeoJsonGeometry
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "LineString";

    [JsonPropertyName("coordinates")]
    public List<double[]> Coordinates { get; set; } = new();
}

/// <summary>
/// Messages from the map JavaScript
/// </summary>
public record MapMessage(string Type, MapMessageData? Data);

public record MapMessageData(
    bool? Success = null,
    string? Error = null,
    double? Lat = null,
    double? Lon = null);

/// <summary>
/// Map initialization options
/// </summary>
public record MapOptions(
    double[]? Center = null,
    int Zoom = 3,
    string Theme = "dark");
