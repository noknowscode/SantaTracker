using System.Collections.Immutable;
using SantaTracker.Models;
using SantaTracker.Services;

namespace SantaTracker.Presentation;

/// <summary>
/// MVUX Model for the main Santa Tracker Dashboard
/// </summary>
public partial record DashboardModel(
    ISantaSimulationService SimulationService,
    IWeatherService WeatherService,
    ICheerScannerService CheerScannerService,
    IThemeService ThemeService)
{
    /// <summary>
    /// Real-time telemetry feed from Santa's sleigh
    /// </summary>
    public IFeed<SantaTelemetry> Telemetry => Feed.AsyncEnumerable(SimulationService.GetTelemetryStream);

    /// <summary>
    /// Reindeer vitals feed (all 9 reindeer)
    /// </summary>
    public IListFeed<ReindeerStatus> ReindeerVitals => ListFeed.AsyncEnumerable(SimulationService.GetReindeerStream);

    /// <summary>
    /// Spirit meter percentage (0-100)
    /// </summary>
    public IFeed<int> SpiritLevel => Feed.AsyncEnumerable(SimulationService.GetSpiritMeterStream);

    /// <summary>
    /// Mission log entries (most recent first)
    /// </summary>
    public IListFeed<MissionLogEntry> MissionLog => ListFeed.AsyncEnumerable(SimulationService.GetMissionLogStream);

    /// <summary>
    /// Current destination from the mission log (for map tracking)
    /// </summary>
    public IFeed<MissionLogEntry?> CurrentDestination => MissionLog
        .AsFeed()
        .Select(entries => entries?.FirstOrDefault(e => e.IsCurrent));

    /// <summary>
    /// Cumulative mission statistics computed from telemetry and mission log
    /// </summary>
    public IFeed<MissionStats> MissionStats => Telemetry.Select(t => new MissionStats(
        TotalStops: (int)(t.ToysDelivered / 50000), // Approximate stops based on toys
        TotalCities: (int)(t.ToysDelivered / 80000), // Approximate cities
        TotalGiftsDelivered: t.ToysDelivered,
        DistanceTraveled: t.DistanceTraveled
    ));

    /// <summary>
    /// Weather alerts feed
    /// </summary>
    public IListFeed<WeatherAlert> WeatherAlerts => ListFeed.Async(async ct => await WeatherService.GetCurrentAlerts(ct));

    /// <summary>
    /// Child's name for cheer scanning (user input)
    /// </summary>
    public IState<string> ChildName => State.Value(this, () => string.Empty);

    /// <summary>
    /// Behavior context for cheer scanning (user input)
    /// </summary>
    public IState<string> BehaviorContext => State.Value(this, () => string.Empty);

    /// <summary>
    /// Flag indicating if a scan is in progress
    /// </summary>
    public IState<bool> IsScanning => State.Value(this, () => false);

    /// <summary>
    /// Result from the most recent cheer scan
    /// </summary>
    public IState<CheerScanResult> CheerResult => State<CheerScanResult>.Empty(this);

    /// <summary>
    /// Current theme (true = dark, false = light)
    /// </summary>
    public IState<bool> IsDarkTheme => State.Value(this, () => ThemeService.IsDark);

    /// <summary>
    /// Command to scan a child's cheer level
    /// </summary>
    public async ValueTask ScanCheer(CancellationToken ct)
    {
        var name = await ChildName;
        var context = await BehaviorContext;

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        await IsScanning.Set(true, ct);

        try
        {
            var result = await CheerScannerService.ScanAsync(name, context ?? string.Empty, ct);
            await CheerResult.UpdateAsync(_ => result, ct);
        }
        finally
        {
            await IsScanning.Set(false, ct);
        }
    }

    /// <summary>
    /// Command to toggle between light and dark theme
    /// </summary>
    public async ValueTask ToggleTheme(CancellationToken ct)
    {
        var isDark = ThemeService.IsDark;
        var newTheme = isDark ? AppTheme.Light : AppTheme.Dark;
        await ThemeService.SetThemeAsync(newTheme);
        await IsDarkTheme.Set(!isDark, ct);
    }

    /// <summary>
    /// Command to clear the cheer scan result
    /// </summary>
    public async ValueTask ClearCheerResult(CancellationToken ct)
    {
        await ChildName.Set(string.Empty, ct);
        await BehaviorContext.Set(string.Empty, ct);
    }
}
