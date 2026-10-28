using SantaTracker.Models;

namespace SantaTracker.Services;

/// <summary>
/// Mock implementation of the Cheer Scanner for demo purposes
/// </summary>
public class MockCheerScannerService : ICheerScannerService
{
    private static readonly Random _random = new();

    // Nice categories and templates
    private static readonly string[] NiceCategories =
    {
        "Golden Heart",
        "Kind Spirit",
        "Growing Goodness",
        "Promising Star",
        "Shining Light"
    };

    private static readonly string[] NiceTemplates =
    {
        "{0} has shown remarkable kindness this year! Their thoughtful actions have brightened many days.",
        "What a wonderful spirit {0} has! Their positive attitude is truly inspiring.",
        "{0} is growing into such a caring person. Every small act of kindness counts!",
        "The holiday spirit shines bright in {0}! Keep spreading that joy.",
        "{0} has been working hard to be their best self. Santa is proud!"
    };

    // Naughty categories and templates (playful, not mean!)
    private static readonly string[] NaughtyCategories =
    {
        "Mischief Maker",
        "Cookie Thief",
        "Snowball Bandit",
        "Elf Impersonator"
    };

    private static readonly string[] NaughtyTemplates =
    {
        "{0} has been caught sneaking extra cookies from the jar! But Santa admires their dedication to snacks.",
        "Our elves report that {0} has been teaching the reindeer silly tricks. Rudolph now does backflips!",
        "{0} started the great snowball war of the century. Impressive aim, though!",
        "{0} was spotted wearing elf ears and trying to sneak into the workshop. Points for creativity!"
    };

    public async Task<CheerScanResult> ScanAsync(string childName, string behaviorContext, CancellationToken ct)
    {
        // Simulate AI processing time
        await Task.Delay(_random.Next(800, 1500), ct);

        // Check for "naughty" keywords in behavior context for fun
        var behaviorLower = behaviorContext?.ToLower() ?? "";
        var hasNaughtyKeywords = behaviorLower.Contains("naughty") ||
                                  behaviorLower.Contains("bad") ||
                                  behaviorLower.Contains("mischief") ||
                                  behaviorLower.Contains("trouble") ||
                                  behaviorLower.Contains("fight") ||
                                  behaviorLower.Contains("stole") ||
                                  behaviorLower.Contains("broke");

        // 80% chance of Nice, 20% chance of Naughty (unless keywords detected)
        var isNice = hasNaughtyKeywords ? _random.Next(100) < 30 : _random.Next(100) < 80;

        string category;
        string statusText;
        string verdict;
        string verdictEmoji;
        int score;

        if (isNice)
        {
            score = _random.Next(75, 100);
            var categoryIndex = score switch
            {
                >= 95 => 0,
                >= 90 => 1,
                >= 85 => 2,
                >= 80 => 3,
                _ => 4
            };
            category = NiceCategories[categoryIndex];
            statusText = string.Format(NiceTemplates[categoryIndex], childName);
            verdict = "NICE";
            verdictEmoji = "✨";

            if (!string.IsNullOrWhiteSpace(behaviorContext))
            {
                statusText += $" Santa especially noticed: {behaviorContext.Trim()}.";
            }
        }
        else
        {
            score = _random.Next(25, 55);
            var categoryIndex = _random.Next(NaughtyCategories.Length);
            category = NaughtyCategories[categoryIndex];
            statusText = string.Format(NaughtyTemplates[categoryIndex], childName);
            verdict = "NAUGHTY";
            verdictEmoji = "🎭";

            // Add redemption note
            statusText += " But don't worry - there's still time to make the Nice list!";
        }

        return new CheerScanResult(score, category, statusText, isNice, verdict, verdictEmoji);
    }
}
