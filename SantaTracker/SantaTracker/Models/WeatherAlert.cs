namespace SantaTracker.Models;

/// <summary>
/// Weather or atmospheric alert affecting the flight path
/// </summary>
public partial record WeatherAlert(
    string Type,
    AlertSeverity Severity,
    string Description,
    string Icon);

/// <summary>
/// Severity level for alerts
/// </summary>
public enum AlertSeverity
{
    Low,
    Medium,
    High
}
