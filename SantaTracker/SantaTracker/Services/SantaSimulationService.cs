using System.Collections.Immutable;
using SantaTracker.Models;

namespace SantaTracker.Services;

/// <summary>
/// Simulates Santa's Christmas Eve journey with data from the brief
/// </summary>
public class SantaSimulationService : ISantaSimulationService
{
    // Use Random.Shared for thread-safety (.NET 6+ best practice)

    // Extended destinations list for continuous rolling
    private static readonly ImmutableList<(string City, string Country, string Coords, double Lat, double Lon)> AllDestinations = ImmutableList.Create(
        ("Mumbai", "India", "19.08°N 72.88°E", 19.0760, 72.8777),
        ("Mexico City", "Mexico", "19.43°N 99.13°W", 19.4326, -99.1332),
        ("Cape Town", "South Africa", "33.92°S 18.42°E", -33.9249, 18.4241),
        ("Tokyo", "Japan", "35.68°N 139.69°E", 35.6762, 139.6503),
        ("Sydney", "Australia", "33.87°S 151.21°E", -33.8688, 151.2093),
        ("London", "UK", "51.51°N 0.13°W", 51.5074, -0.1278),
        ("New York", "USA", "40.71°N 74.01°W", 40.7128, -74.0060),
        ("Paris", "France", "48.86°N 2.35°E", 48.8566, 2.3522),
        ("Berlin", "Germany", "52.52°N 13.40°E", 52.5200, 13.4050),
        ("Toronto", "Canada", "43.65°N 79.38°W", 43.6532, -79.3832),
        ("São Paulo", "Brazil", "23.55°S 46.63°W", -23.5505, -46.6333),
        ("Moscow", "Russia", "55.76°N 37.62°E", 55.7558, 37.6173),
        ("Beijing", "China", "39.90°N 116.41°E", 39.9042, 116.4074),
        ("Dubai", "UAE", "25.20°N 55.27°E", 25.2048, 55.2708),
        ("Singapore", "Singapore", "1.35°N 103.82°E", 1.3521, 103.8198),
        ("Stockholm", "Sweden", "59.33°N 18.07°E", 59.3293, 18.0686),
        ("Oslo", "Norway", "59.91°N 10.75°E", 59.9139, 10.7522),
        ("Helsinki", "Finland", "60.17°N 24.94°E", 60.1699, 24.9384),
        ("Reykjavik", "Iceland", "64.15°N 21.94°W", 64.1466, -21.9426),
        ("Amsterdam", "Netherlands", "52.37°N 4.90°E", 52.3676, 4.9041)
    );

    // 9 reindeer - the complete team
    private static readonly ImmutableList<(string Name, string Emoji)> ReindeerData = ImmutableList.Create(
        ("Dasher", "⚡"),
        ("Dancer", "💃"),
        ("Prancer", "🦌"),
        ("Vixen", "🦊"),
        ("Comet", "☄️"),
        ("Cupid", "💘"),
        ("Donner", "🌩️"),
        ("Blitzen", "⚡"),
        ("Rudolph", "🔴")
    );

    private int _currentDestIndex = 0;
    private int _missionLogIndex = 0;

    // Starting values from the brief
    private long _toysDelivered = 421_405_244;
    private long _cookiesEaten = 1_205_112;
    private double _distanceTraveled = 186_954;
    private readonly List<MissionLogEntry> _missionLog = new();

    public SantaSimulationService()
    {
        // Initialize mission log with first 5 destinations (with realistic toy counts)
        for (int i = 0; i < 5; i++)
        {
            var dest = AllDestinations[i];
            var toysForCity = Random.Shared.Next(50_000, 500_000);
            _missionLog.Add(new MissionLogEntry(
                dest.City,
                dest.Country,
                dest.Coords,
                dest.Lat,
                dest.Lon,
                toysForCity,
                DateTimeOffset.Now.AddMinutes(-i * 15),
                IsCurrent: i == 0
            ));
        }
        _missionLogIndex = 5;
    }

    public async IAsyncEnumerable<SantaTelemetry> GetTelemetryStream(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var dest = AllDestinations[_currentDestIndex];

            // Slowly increment counters
            var toysThisStop = Random.Shared.Next(1000, 5000);
            var cookiesThisStop = Random.Shared.Next(10, 50);
            var distanceThisLeg = Random.Shared.Next(50, 200);

            _toysDelivered += toysThisStop;
            _cookiesEaten += cookiesThisStop;
            _distanceTraveled += distanceThisLeg;

            var telemetry = new SantaTelemetry(
                _toysDelivered,
                _cookiesEaten,
                _distanceTraveled,
                new GeoCoordinate(dest.Lat, dest.Lon),
                dest.City,
                dest.Country,
                DateTimeOffset.Now
            );

            yield return telemetry;

            // Move to next destination slowly
            if (Random.Shared.Next(10) == 0)
            {
                _currentDestIndex = (_currentDestIndex + 1) % AllDestinations.Count;
            }

            // Wait before next update (2-4 seconds)
            await Task.Delay(Random.Shared.Next(2000, 4000), ct);
        }
    }

    public async IAsyncEnumerable<IImmutableList<ReindeerStatus>> GetReindeerStream(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        // Energy levels - visible range (60-90%) for all 9 reindeer
        var energyLevels = new List<int> { 78, 82, 71, 85, 65, 79, 73, 88, 92 };
        // States for all 9 reindeer
        var states = new List<ReindeerState>
        {
            ReindeerState.Zooming, // Dasher
            ReindeerState.OK,      // Dancer
            ReindeerState.Zooming, // Prancer
            ReindeerState.OK,      // Vixen
            ReindeerState.Tired,   // Comet
            ReindeerState.OK,      // Cupid
            ReindeerState.Zooming, // Donner
            ReindeerState.Zooming, // Blitzen
            ReindeerState.Zooming  // Rudolph
        };

        while (!ct.IsCancellationRequested)
        {
            var reindeer = ReindeerData.Select((data, index) =>
            {
                // Energy fluctuation within visible range
                energyLevels[index] += Random.Shared.Next(-3, 4);
                energyLevels[index] = Math.Clamp(energyLevels[index], 55, 95);

                // Occasionally change state
                if (Random.Shared.Next(20) == 0)
                {
                    states[index] = (ReindeerState)Random.Shared.Next(3);
                }

                return new ReindeerStatus(
                    data.Name,
                    data.Emoji,
                    energyLevels[index],
                    states[index],
                    IsLeader: data.Name == "Rudolph"
                );
            }).ToImmutableList();

            yield return reindeer;

            // Update every 2 seconds
            await Task.Delay(2000, ct);
        }
    }

    public async IAsyncEnumerable<int> GetSpiritMeterStream(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var spiritLevel = 75;

        while (!ct.IsCancellationRequested)
        {
            spiritLevel += Random.Shared.Next(-3, 5);
            spiritLevel = Math.Clamp(spiritLevel, 50, 100);

            yield return spiritLevel;

            await Task.Delay(1500, ct);
        }
    }

    public async IAsyncEnumerable<IImmutableList<MissionLogEntry>> GetMissionLogStream(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            // Return current mission log
            yield return _missionLog.ToImmutableList();

            // Wait before adding next destination
            await Task.Delay(8000, ct);

            // Add a new destination and roll the timeline
            if (_missionLogIndex < AllDestinations.Count)
            {
                var newDest = AllDestinations[_missionLogIndex];
                var toysForCity = Random.Shared.Next(80_000, 400_000);

                // Mark all existing as not current
                for (int i = 0; i < _missionLog.Count; i++)
                {
                    if (_missionLog[i].IsCurrent)
                    {
                        _missionLog[i] = _missionLog[i] with { IsCurrent = false };
                    }
                }

                // Add new destination as current
                _missionLog.Insert(0, new MissionLogEntry(
                    newDest.City,
                    newDest.Country,
                    newDest.Coords,
                    newDest.Lat,
                    newDest.Lon,
                    toysForCity,
                    DateTimeOffset.Now,
                    IsCurrent: true
                ));

                // Keep only last 6 entries for display
                while (_missionLog.Count > 6)
                {
                    _missionLog.RemoveAt(_missionLog.Count - 1);
                }

                _missionLogIndex++;

                // Wrap around if we've gone through all destinations
                if (_missionLogIndex >= AllDestinations.Count)
                {
                    _missionLogIndex = 0;
                }
            }
        }
    }
}
