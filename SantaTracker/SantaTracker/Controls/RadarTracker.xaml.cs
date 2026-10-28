using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SantaTracker.Controls;

public sealed partial class RadarTracker : UserControl
{
    public RadarTracker()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SweepAnimation.Begin();
        PulseAnimation.Begin();
    }

    public static readonly DependencyProperty LatitudeProperty =
        DependencyProperty.Register(nameof(Latitude), typeof(double), typeof(RadarTracker), new PropertyMetadata(0.0, OnCoordinateChanged));

    public double Latitude
    {
        get => (double)GetValue(LatitudeProperty);
        set => SetValue(LatitudeProperty, value);
    }

    public static readonly DependencyProperty LongitudeProperty =
        DependencyProperty.Register(nameof(Longitude), typeof(double), typeof(RadarTracker), new PropertyMetadata(0.0, OnCoordinateChanged));

    public double Longitude
    {
        get => (double)GetValue(LongitudeProperty);
        set => SetValue(LongitudeProperty, value);
    }

    public string CoordinateDisplay =>
        $"{Math.Abs(Latitude):F2}°{(Latitude >= 0 ? "N" : "S")} {Math.Abs(Longitude):F2}°{(Longitude >= 0 ? "E" : "W")}";

    private static void OnCoordinateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RadarTracker tracker)
        {
            tracker.UpdateDotPosition();
            tracker.Bindings.Update();
        }
    }

    private void UpdateDotPosition()
    {
        // Map lat/lon to position within the radar circle
        // Latitude: -90 to 90 -> Y position
        // Longitude: -180 to 180 -> X position
        var normalizedX = (Longitude + 180) / 360.0;
        var normalizedY = (90 - Latitude) / 180.0;

        // Scale to radar bounds (inner area ~200px)
        var offsetX = (normalizedX - 0.5) * 160;
        var offsetY = (normalizedY - 0.5) * 160;

        SantaDotContainer.RenderTransform = new Microsoft.UI.Xaml.Media.TranslateTransform
        {
            X = offsetX,
            Y = offsetY
        };
    }
}
