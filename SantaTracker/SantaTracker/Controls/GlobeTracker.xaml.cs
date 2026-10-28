using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace SantaTracker.Controls;

public sealed partial class GlobeTracker : UserControl
{
    public GlobeTracker()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RotationAnimation.Begin();
        PulseAnimation.Begin();
        GlowAnimation.Begin();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        RotationAnimation.Stop();
        PulseAnimation.Stop();
        GlowAnimation.Stop();
    }

    public static readonly DependencyProperty LatitudeProperty =
        DependencyProperty.Register(nameof(Latitude), typeof(double), typeof(GlobeTracker), new PropertyMetadata(0.0, OnCoordinateChanged));

    public double Latitude
    {
        get => (double)GetValue(LatitudeProperty);
        set => SetValue(LatitudeProperty, value);
    }

    public static readonly DependencyProperty LongitudeProperty =
        DependencyProperty.Register(nameof(Longitude), typeof(double), typeof(GlobeTracker), new PropertyMetadata(0.0, OnCoordinateChanged));

    public double Longitude
    {
        get => (double)GetValue(LongitudeProperty);
        set => SetValue(LongitudeProperty, value);
    }

    public string CoordinateDisplay =>
        $"{Math.Abs(Latitude):F2}°{(Latitude >= 0 ? "N" : "S")} {Math.Abs(Longitude):F2}°{(Longitude >= 0 ? "E" : "W")}";

    private static void OnCoordinateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GlobeTracker tracker)
        {
            tracker.UpdateMarkerPosition();
            tracker.Bindings.Update();
        }
    }

    private void UpdateMarkerPosition()
    {
        // Map lat/lon to position on the globe face
        // Globe diameter is 220px, centered in 260px container
        // Center offset is 20px (260-220)/2

        // Normalize coordinates
        // Longitude: -180 to 180 -> X position (-80 to 80 from center)
        // Latitude: -90 to 90 -> Y position (80 to -80 from center, inverted)

        var normalizedX = Longitude / 180.0;  // -1 to 1
        var normalizedY = -Latitude / 90.0;   // -1 to 1 (inverted for screen coords)

        // Scale to visible globe area (about 80px radius for good visibility)
        var offsetX = normalizedX * 70;
        var offsetY = normalizedY * 70;

        SantaMarkerContainer.RenderTransform = new TranslateTransform
        {
            X = offsetX,
            Y = offsetY
        };
    }
}
