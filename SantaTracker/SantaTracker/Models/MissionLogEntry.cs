namespace SantaTracker.Models;

/// <summary>
/// A destination in Santa's route queue
/// </summary>
public partial record MissionLogEntry(
    string City,
    string Country,
    string Coordinates,
    double Latitude,
    double Longitude,
    long ToysDelivered,
    DateTimeOffset Timestamp,
    bool IsCurrent = false)
{
    /// <summary>
    /// Gets the country flag emoji based on country name
    /// </summary>
    public string CountryFlag => Country switch
    {
        "Japan" => "🇯🇵",
        "Australia" => "🇦🇺",
        "India" => "🇮🇳",
        "Germany" => "🇩🇪",
        "France" => "🇫🇷",
        "Brazil" => "🇧🇷",
        "Canada" => "🇨🇦",
        "USA" => "🇺🇸",
        "United States" => "🇺🇸",
        "UK" => "🇬🇧",
        "United Kingdom" => "🇬🇧",
        "China" => "🇨🇳",
        "Russia" => "🇷🇺",
        "Mexico" => "🇲🇽",
        "Spain" => "🇪🇸",
        "Italy" => "🇮🇹",
        "South Korea" => "🇰🇷",
        "Netherlands" => "🇳🇱",
        "Sweden" => "🇸🇪",
        "Norway" => "🇳🇴",
        "Finland" => "🇫🇮",
        "Denmark" => "🇩🇰",
        "Switzerland" => "🇨🇭",
        "Austria" => "🇦🇹",
        "Poland" => "🇵🇱",
        "Belgium" => "🇧🇪",
        "Portugal" => "🇵🇹",
        "Greece" => "🇬🇷",
        "Ireland" => "🇮🇪",
        "New Zealand" => "🇳🇿",
        "Singapore" => "🇸🇬",
        "Thailand" => "🇹🇭",
        "Indonesia" => "🇮🇩",
        "Philippines" => "🇵🇭",
        "Vietnam" => "🇻🇳",
        "Malaysia" => "🇲🇾",
        "South Africa" => "🇿🇦",
        "Egypt" => "🇪🇬",
        "Nigeria" => "🇳🇬",
        "Kenya" => "🇰🇪",
        "Argentina" => "🇦🇷",
        "Chile" => "🇨🇱",
        "Colombia" => "🇨🇴",
        "Peru" => "🇵🇪",
        "Iceland" => "🇮🇸",
        _ => "🌍"
    };

    /// <summary>
    /// Gets the formatted arrival time (e.g., "23:45")
    /// </summary>
    public string ArrivalTime => Timestamp.ToString("HH:mm");

    /// <summary>
    /// Gets formatted presents count (e.g., "1,234")
    /// </summary>
    public string FormattedPresents => ToysDelivered.ToString("N0");

    /// <summary>
    /// Returns true if this stop has been visited (not current, implies past)
    /// For timeline display: shows filled red circle (●)
    /// </summary>
    public bool IsVisited => !IsCurrent;

    /// <summary>
    /// Returns true if this stop has NOT been visited (future stop)
    /// For timeline display: shows faded empty circle (○)
    /// Note: Currently all mission log entries are visited or current
    /// </summary>
    public bool IsNotVisited => false;

    /// <summary>
    /// Content opacity for Aegis HUD design
    /// Current items: 100%, Visited items: 40%
    /// </summary>
    public double ContentOpacity => IsCurrent ? 1.0 : 0.4;
}
