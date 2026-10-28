using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace SantaTracker.Controls;

public sealed partial class MagicOMeter : UserControl
{
    private DispatcherTimer? _updateTimer;
    private int _currentMagicValue = 847;

    // Status values with their colors
    private readonly (string Value, string Status)[] _moraleOptions =
    {
        ("Excellent", "green"),
        ("High", "green"),
        ("Good", "gold"),
        ("Moderate", "gold")
    };

    private readonly (string Value, string Status)[] _productivityOptions =
    {
        ("98.7%", "green"),
        ("97.2%", "green"),
        ("95.8%", "green"),
        ("94.1%", "gold")
    };

    private readonly (string Value, string Status)[] _cookieOptions =
    {
        ("Abundant", "green"),
        ("Plentiful", "green"),
        ("Moderate", "gold"),
        ("Low", "crimson")
    };

    private readonly (string Value, string Status)[] _snowGlobeOptions =
    {
        ("Enchanted", "purple"),
        ("Magical", "purple"),
        ("Stable", "green"),
        ("Fluctuating", "gold")
    };

    public MagicOMeter()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Set initial needle position
        UpdateNeedlePosition(_currentMagicValue);

        // Start automatic updates
        _updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1500)
        };
        _updateTimer.Tick += OnUpdateTimerTick;
        _updateTimer.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _updateTimer?.Stop();
        _updateTimer = null;
    }

    private void OnUpdateTimerTick(object? sender, object e)
    {
        // Fluctuate magic value (targeting 750-950 range)
        var change = Random.Shared.Next(-15, 20);
        _currentMagicValue = Math.Clamp(_currentMagicValue + change, 600, 999);

        // Update display
        MagicValueText.Text = _currentMagicValue.ToString();
        UpdateNeedlePosition(_currentMagicValue);

        // Occasionally update status values
        if (Random.Shared.Next(5) == 0)
        {
            UpdateStatusValues();
        }
    }

    private void UpdateNeedlePosition(int magicValue)
    {
        // Map magic value (600-999) to angle (-80 to +80 degrees)
        // At 600: -80 degrees (left), at 999: +80 degrees (right)
        var normalized = (magicValue - 600) / 399.0; // 0.0 to 1.0
        var angle = -80 + (normalized * 160); // -80 to +80

        NeedleRotation.Angle = angle;
    }

    private void UpdateStatusValues()
    {
        // Randomly update one of the status values
        var statusToUpdate = Random.Shared.Next(4);

        switch (statusToUpdate)
        {
            case 0:
                var morale = _moraleOptions[Random.Shared.Next(_moraleOptions.Length)];
                ReindeerMoraleText.Text = morale.Value;
                ReindeerMoraleText.Foreground = GetStatusBrush(morale.Status);
                break;
            case 1:
                var productivity = _productivityOptions[Random.Shared.Next(_productivityOptions.Length)];
                ElfProductivityText.Text = productivity.Value;
                ElfProductivityText.Foreground = GetStatusBrush(productivity.Status);
                break;
            case 2:
                var cookies = _cookieOptions[Random.Shared.Next(_cookieOptions.Length)];
                CookieReservesText.Text = cookies.Value;
                CookieReservesText.Foreground = GetStatusBrush(cookies.Status);
                break;
            case 3:
                var snowGlobe = _snowGlobeOptions[Random.Shared.Next(_snowGlobeOptions.Length)];
                SnowGlobeText.Text = snowGlobe.Value;
                SnowGlobeText.Foreground = GetStatusBrush(snowGlobe.Status);
                break;
        }
    }

    private Brush GetStatusBrush(string status)
    {
        return status switch
        {
            "green" => (Brush)Resources["ScannerNiceGreenBrush"]
                ?? Application.Current.Resources["ScannerNiceGreenBrush"] as Brush
                ?? new SolidColorBrush(Microsoft.UI.Colors.Green),
            "gold" => (Brush)Resources["ScannerGoldBrush"]
                ?? Application.Current.Resources["ScannerGoldBrush"] as Brush
                ?? new SolidColorBrush(Microsoft.UI.Colors.Gold),
            "crimson" => (Brush)Resources["ScannerCrimsonBrush"]
                ?? Application.Current.Resources["ScannerCrimsonBrush"] as Brush
                ?? new SolidColorBrush(Microsoft.UI.Colors.Crimson),
            "purple" => (Brush)Resources["MagicPurpleBrush"]
                ?? Application.Current.Resources["MagicPurpleBrush"] as Brush
                ?? new SolidColorBrush(Microsoft.UI.Colors.Purple),
            _ => new SolidColorBrush(Microsoft.UI.Colors.White)
        };
    }
}
