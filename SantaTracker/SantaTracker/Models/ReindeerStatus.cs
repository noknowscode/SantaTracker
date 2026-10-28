namespace SantaTracker.Models;

/// <summary>
/// Status of a single reindeer in Santa's team
/// </summary>
public partial record ReindeerStatus(
    string Name,
    string Emoji,
    int EnergyLevel,
    ReindeerState State,
    bool IsLeader);

/// <summary>
/// Possible states for a reindeer
/// </summary>
public enum ReindeerState
{
    OK,
    Tired,
    Zooming
}
