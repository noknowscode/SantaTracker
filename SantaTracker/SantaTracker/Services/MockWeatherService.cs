using System.Collections.Immutable;
using SantaTracker.Models;

namespace SantaTracker.Services;

/// <summary>
/// Mock weather service with static alerts
/// </summary>
public class MockWeatherService : IWeatherService
{
    private static readonly ImmutableList<WeatherAlert> MockAlerts = ImmutableList.Create(
        new WeatherAlert(
            "Snow",
            AlertSeverity.Low,
            "Light flurries over Northern Europe. Visibility good.",
            "\uE9C9" // Snowflake icon
        ),
        new WeatherAlert(
            "Wind",
            AlertSeverity.Medium,
            "Crosswinds at 25kt over the Atlantic. Minor turbulence expected.",
            "\uE9CA" // Wind icon
        ),
        new WeatherAlert(
            "Air Traffic",
            AlertSeverity.Low,
            "Commercial traffic light due to holiday hours. Clear skies ahead.",
            "\uE709" // Airplane icon
        )
    );

    public Task<IImmutableList<WeatherAlert>> GetCurrentAlerts(CancellationToken ct)
    {
        return Task.FromResult<IImmutableList<WeatherAlert>>(MockAlerts);
    }
}
