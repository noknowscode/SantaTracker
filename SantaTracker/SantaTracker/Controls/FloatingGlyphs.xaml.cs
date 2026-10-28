using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SantaTracker.Controls;

public sealed partial class FloatingGlyphs : UserControl
{
    public FloatingGlyphs()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Start all floating animations (3 parallax layers)
        DistantLayerAnim.Begin();
        MidLayerAnim.Begin();
        ForeLayerAnim.Begin();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Stop all animations to free resources (Uno Platform best practice)
        DistantLayerAnim.Stop();
        MidLayerAnim.Stop();
        ForeLayerAnim.Stop();
    }
}
