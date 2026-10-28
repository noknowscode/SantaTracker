using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SantaTracker.Models;

namespace SantaTracker.Controls;

public sealed partial class ReindeerCard : UserControl
{
    public ReindeerCard()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ReindeerNameProperty =
        DependencyProperty.Register(nameof(ReindeerName), typeof(string), typeof(ReindeerCard), new PropertyMetadata(string.Empty, OnPropertyChanged));

    public string ReindeerName
    {
        get => (string)GetValue(ReindeerNameProperty);
        set => SetValue(ReindeerNameProperty, value);
    }

    public static readonly DependencyProperty EnergyLevelProperty =
        DependencyProperty.Register(nameof(EnergyLevel), typeof(int), typeof(ReindeerCard), new PropertyMetadata(100, OnPropertyChanged));

    public int EnergyLevel
    {
        get => (int)GetValue(EnergyLevelProperty);
        set => SetValue(EnergyLevelProperty, value);
    }

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(ReindeerState), typeof(ReindeerCard), new PropertyMetadata(ReindeerState.OK, OnPropertyChanged));

    public ReindeerState State
    {
        get => (ReindeerState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly DependencyProperty IsLeaderProperty =
        DependencyProperty.Register(nameof(IsLeader), typeof(bool), typeof(ReindeerCard), new PropertyMetadata(false, OnPropertyChanged));

    public bool IsLeader
    {
        get => (bool)GetValue(IsLeaderProperty);
        set => SetValue(IsLeaderProperty, value);
    }

    public static readonly DependencyProperty EmojiProperty =
        DependencyProperty.Register(nameof(Emoji), typeof(string), typeof(ReindeerCard), new PropertyMetadata("", OnPropertyChanged));

    public string Emoji
    {
        get => (string)GetValue(EmojiProperty);
        set => SetValue(EmojiProperty, value);
    }

    // Computed properties

    /// <summary>
    /// Returns uppercase display name
    /// </summary>
    public string DisplayName => ReindeerName?.ToUpper() ?? "";

    /// <summary>
    /// Returns the deer emoji for all reindeer (Rudolph shows glowing nose instead)
    /// </summary>
    public string IconEmoji => ReindeerName?.ToLower() == "rudolph"
        ? "" // Empty - Rudolph shows the glowing red nose ellipse instead
        : "\U0001F98C"; // 🦌 Deer emoji for all reindeer

    /// <summary>
    /// Visibility for Rudolph's special glowing nose
    /// </summary>
    public Visibility IsRudolphVisibility =>
        ReindeerName?.ToLower() == "rudolph" ? Visibility.Visible : Visibility.Collapsed;

    public string StatusText => State.ToString().ToUpper();

    public string EnergyText => $"{EnergyLevel}%";

    /// <summary>
    /// Width of the progress bar fill (0-80 based on EnergyLevel percentage)
    /// </summary>
    public double ProgressWidth => Math.Max(0, Math.Min(80, EnergyLevel * 0.8));

    /// <summary>
    /// Width of the progress bar fill for compact layout (0-60 based on EnergyLevel percentage)
    /// </summary>
    public double ProgressWidthSmall => Math.Max(0, Math.Min(60, EnergyLevel * 0.6));

    public Brush StatusColor => State switch
    {
        ReindeerState.OK => (Brush)Application.Current.Resources["FleetStatusOkBrush"],
        ReindeerState.Tired => (Brush)Application.Current.Resources["FleetStatusTiredBrush"],
        ReindeerState.Zooming => (Brush)Application.Current.Resources["FleetStatusZoomingBrush"],
        _ => (Brush)Application.Current.Resources["FleetStatusOkBrush"]
    };

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ReindeerCard card)
        {
            card.Bindings.Update();
        }
    }
}
