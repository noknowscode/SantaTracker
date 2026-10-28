using SantaTracker.Models;

namespace SantaTracker.Services;

/// <summary>
/// AI-powered service for scanning children's cheer levels
/// </summary>
public interface ICheerScannerService
{
    /// <summary>
    /// Scan a child's cheer level based on their name and behavior context
    /// </summary>
    Task<CheerScanResult> ScanAsync(string childName, string behaviorContext, CancellationToken ct);
}
