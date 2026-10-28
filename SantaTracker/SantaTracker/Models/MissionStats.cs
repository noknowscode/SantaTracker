namespace SantaTracker.Models;

/// <summary>
/// Aggregate statistics for the mission log display
/// </summary>
public record MissionStats(
    int TotalStops,
    int TotalCities,
    long TotalGiftsDelivered,
    double DistanceTraveled)
{
    /// <summary>
    /// Formatted distance (e.g., "24.8K")
    /// </summary>
    public string FormattedDistance
    {
        get
        {
            if (DistanceTraveled >= 1000)
                return $"{DistanceTraveled / 1000:F1}K";
            return DistanceTraveled.ToString("N0");
        }
    }

    /// <summary>
    /// Formatted gifts count
    /// </summary>
    public string FormattedGifts => TotalGiftsDelivered.ToString("N0");
}
