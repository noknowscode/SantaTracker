using System.Collections.Immutable;
using SantaTracker.Models;

namespace SantaTracker.Services;

/// <summary>
/// Service providing real-time simulation of Santa's journey
/// </summary>
public interface ISantaSimulationService
{
    /// <summary>
    /// Stream of telemetry updates
    /// </summary>
    IAsyncEnumerable<SantaTelemetry> GetTelemetryStream(CancellationToken ct);

    /// <summary>
    /// Stream of reindeer status updates
    /// </summary>
    IAsyncEnumerable<IImmutableList<ReindeerStatus>> GetReindeerStream(CancellationToken ct);

    /// <summary>
    /// Stream of spirit meter updates (0-100)
    /// </summary>
    IAsyncEnumerable<int> GetSpiritMeterStream(CancellationToken ct);

    /// <summary>
    /// Stream of mission log updates (FILO)
    /// </summary>
    IAsyncEnumerable<IImmutableList<MissionLogEntry>> GetMissionLogStream(CancellationToken ct);
}
