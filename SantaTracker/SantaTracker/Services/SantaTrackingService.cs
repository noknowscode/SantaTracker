using SantaTracker.Models;

namespace SantaTracker.Services;

/// <summary>
/// Interface for Santa tracking service
/// </summary>
public interface ISantaTrackingService
{
    event EventHandler<SantaPosition>? PositionChanged;
    event EventHandler<List<GeoPoint>>? RouteUpdated;

    SantaPosition CurrentPosition { get; }
    List<GeoPoint> FullRoute { get; }
    List<GeoPoint> VisitedRoute { get; }
    List<Destination> Destinations { get; }

    void Start();
    void Stop();
}

/// <summary>
/// Represents a destination on Santa's route
/// </summary>
public record Destination(
    string Name,
    string Country,
    GeoPoint Location,
    bool IsVisited = false,
    string? ArrivalTime = null);

/// <summary>
/// Mock Santa tracking service that simulates Santa's journey around the world
/// </summary>
public class MockSantaTrackingService : ISantaTrackingService
{
    private readonly Timer _timer;
    private int _currentDestinationIndex;
    private double _progressToNext;
    private readonly Random _random = new();

    public event EventHandler<SantaPosition>? PositionChanged;
    public event EventHandler<List<GeoPoint>>? RouteUpdated;

    public SantaPosition CurrentPosition { get; private set; } = null!;
    public List<GeoPoint> FullRoute { get; private set; } = new();
    public List<GeoPoint> VisitedRoute { get; private set; } = new();
    public List<Destination> Destinations { get; private set; }

    public MockSantaTrackingService()
    {
        // Santa's Christmas Eve route (simplified)
        Destinations = new List<Destination>
        {
            new("North Pole", "Arctic", new GeoPoint(90.0, 0.0), true, "00:00"),
            new("Provideniya", "Russia", new GeoPoint(64.4, -173.2), true, "00:45"),
            new("Tokyo", "Japan", new GeoPoint(35.6762, 139.6503), true, "02:30"),
            new("Beijing", "China", new GeoPoint(39.9042, 116.4074), true, "03:15"),
            new("Mumbai", "India", new GeoPoint(19.0760, 72.8777), false, "04:30"),
            new("Dubai", "UAE", new GeoPoint(25.2048, 55.2708), false, "05:15"),
            new("Moscow", "Russia", new GeoPoint(55.7558, 37.6173), false, "06:00"),
            new("Berlin", "Germany", new GeoPoint(52.5200, 13.4050), false, "06:45"),
            new("Paris", "France", new GeoPoint(48.8566, 2.3522), false, "07:15"),
            new("London", "UK", new GeoPoint(51.5074, -0.1278), false, "07:45"),
            new("Reykjavik", "Iceland", new GeoPoint(64.1466, -21.9426), false, "08:30"),
            new("New York", "USA", new GeoPoint(40.7128, -74.0060), false, "09:30"),
            new("Chicago", "USA", new GeoPoint(41.8781, -87.6298), false, "10:15"),
            new("Denver", "USA", new GeoPoint(39.7392, -104.9903), false, "11:00"),
            new("Los Angeles", "USA", new GeoPoint(34.0522, -118.2437), false, "11:45"),
            new("Honolulu", "USA", new GeoPoint(21.3069, -157.8583), false, "13:00"),
            new("North Pole", "Arctic", new GeoPoint(90.0, 0.0), false, "23:59")
        };

        // Build full route
        FullRoute = Destinations.Select(d => d.Location).ToList();

        // Start at a random position along the route (for demo)
        _currentDestinationIndex = 4; // Start heading towards Mumbai
        _progressToNext = 0.3;

        // Calculate initial position
        UpdateCurrentPosition();

        // Build visited route
        UpdateVisitedRoute();

        _timer = new Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);
    }

    public void Start()
    {
        _timer.Change(0, 2000); // Update every 2 seconds
    }

    public void Stop()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    private void OnTick(object? state)
    {
        // Progress Santa along the route
        _progressToNext += 0.05 + (_random.NextDouble() * 0.03);

        if (_progressToNext >= 1.0)
        {
            // Arrived at next destination
            _progressToNext = 0;
            _currentDestinationIndex++;

            if (_currentDestinationIndex >= Destinations.Count - 1)
            {
                _currentDestinationIndex = 0; // Loop back
            }

            // Mark as visited
            var destinations = Destinations.ToList();
            destinations[_currentDestinationIndex] = destinations[_currentDestinationIndex] with { IsVisited = true };
            Destinations = destinations;

            UpdateVisitedRoute();
        }

        UpdateCurrentPosition();
        PositionChanged?.Invoke(this, CurrentPosition);
    }

    private void UpdateCurrentPosition()
    {
        if (_currentDestinationIndex >= Destinations.Count - 1)
        {
            CurrentPosition = new SantaPosition(
                Destinations[0].Location.Latitude,
                Destinations[0].Location.Longitude,
                0, 0,
                Destinations[0].Name,
                Destinations[0].ArrivalTime);
            return;
        }

        var from = Destinations[_currentDestinationIndex].Location;
        var to = Destinations[_currentDestinationIndex + 1].Location;

        // Interpolate position
        var lat = from.Latitude + (to.Latitude - from.Latitude) * _progressToNext;
        var lon = from.Longitude + (to.Longitude - from.Longitude) * _progressToNext;

        // Calculate heading (bearing)
        var heading = CalculateBearing(from, to);

        // Random speed variation
        var speed = 2500 + (_random.NextDouble() * 500); // ~2500-3000 km/h

        var nextDest = Destinations[_currentDestinationIndex + 1];

        CurrentPosition = new SantaPosition(
            lat,
            lon,
            heading,
            speed,
            nextDest.Name,
            nextDest.ArrivalTime);
    }

    private void UpdateVisitedRoute()
    {
        VisitedRoute = Destinations
            .Take(_currentDestinationIndex + 1)
            .Select(d => d.Location)
            .ToList();

        // Add current interpolated position
        if (CurrentPosition != null)
        {
            VisitedRoute.Add(CurrentPosition.ToGeoPoint());
        }

        RouteUpdated?.Invoke(this, VisitedRoute);
    }

    private static double CalculateBearing(GeoPoint from, GeoPoint to)
    {
        var lat1 = from.Latitude * Math.PI / 180;
        var lat2 = to.Latitude * Math.PI / 180;
        var dLon = (to.Longitude - from.Longitude) * Math.PI / 180;

        var y = Math.Sin(dLon) * Math.Cos(lat2);
        var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

        var bearing = Math.Atan2(y, x) * 180 / Math.PI;
        return (bearing + 360) % 360;
    }
}
