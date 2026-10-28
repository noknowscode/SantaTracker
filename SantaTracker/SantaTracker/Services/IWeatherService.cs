using System.Collections.Immutable;
using SantaTracker.Models;

namespace SantaTracker.Services;

/// <summary>
/// Service providing weather and atmospheric alerts
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Get current weather alerts
    /// </summary>
    Task<IImmutableList<WeatherAlert>> GetCurrentAlerts(CancellationToken ct);
}
