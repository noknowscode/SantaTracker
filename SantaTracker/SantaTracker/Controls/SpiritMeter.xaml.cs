using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace SantaTracker.Controls;

public sealed partial class SpiritMeter : UserControl
{
    // Circumference of the circle for dash calculations
    private const double Circumference = Math.PI * 120; // ~377

    public SpiritMeter()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(int), typeof(SpiritMeter), new PropertyMetadata(75, OnValueChanged));

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public DoubleCollection DashArray
    {
        get
        {
            var progress = Value / 100.0;
            var dashLength = Circumference * progress;
            var gapLength = Circumference * (1 - progress);
            return new DoubleCollection { dashLength / 12, gapLength / 12 };
        }
    }

    public string StatusText => Value switch
    {
        >= 90 => "Absolutely Magical!",
        >= 75 => "Spirit is Strong",
        >= 60 => "Good Vibes",
        >= 45 => "Building Momentum",
        _ => "Needs More Cheer"
    };

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SpiritMeter meter)
        {
            meter.Bindings.Update();
        }
    }
}
