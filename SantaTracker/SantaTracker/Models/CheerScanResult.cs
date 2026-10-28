namespace SantaTracker.Models;

/// <summary>
/// Result from the AI Cheer Scanner
/// </summary>
public partial record CheerScanResult(
    int Score,
    string Category,
    string StatusText,
    bool IsNice,
    string Verdict,
    string VerdictEmoji);

/// <summary>
/// Input for the Cheer Scanner
/// </summary>
public partial record CheerScanRequest(
    string ChildName,
    string BehaviorContext);
