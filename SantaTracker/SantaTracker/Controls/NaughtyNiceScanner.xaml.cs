using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace SantaTracker.Controls;

public sealed partial class NaughtyNiceScanner : UserControl
{
    private static readonly string[] NiceMessages =
    [
        "Outstanding kindness detected!",
        "Exceptional sharing behavior noted.",
        "Homework completion: Excellent!",
        "Sibling harmony levels: Outstanding",
        "Vegetable consumption: Above average!"
    ];

    private static readonly string[] NaughtyMessages =
    [
        "Minor cookie jar incident detected.",
        "Bedtime negotiations exceeded limits.",
        "Sibling dispute ratio: Elevated",
        "Room cleanliness: Needs improvement",
        "Screen time boundaries: Stretched"
    ];

    public NaughtyNiceScanner()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
    }

    #region Dependency Properties

    public static readonly DependencyProperty ChildNameProperty =
        DependencyProperty.Register(nameof(ChildName), typeof(string), typeof(NaughtyNiceScanner),
            new PropertyMetadata(string.Empty));

    public string ChildName
    {
        get => (string)GetValue(ChildNameProperty);
        set => SetValue(ChildNameProperty, value);
    }

    public static readonly DependencyProperty ScanCommandProperty =
        DependencyProperty.Register(nameof(ScanCommand), typeof(object), typeof(NaughtyNiceScanner),
            new PropertyMetadata(null));

    public object ScanCommand
    {
        get => GetValue(ScanCommandProperty);
        set => SetValue(ScanCommandProperty, value);
    }

    #endregion

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Start radar animations
        if (Resources.TryGetValue("RadarSweepAnimation", out var sweepAnim) && sweepAnim is Storyboard sweepStoryboard)
        {
            sweepStoryboard.Begin();
        }

        if (Resources.TryGetValue("CenterPulseAnimation", out var pulseAnim) && pulseAnim is Storyboard pulseStoryboard)
        {
            pulseStoryboard.Begin();
        }
    }

    private async void OnScanClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ChildName))
        {
            NameInput.Focus(FocusState.Programmatic);
            return;
        }

        // Hide idle, show scanning state
        IdleText.Text = "Scanning...";
        StatusContainer.Visibility = Visibility.Visible;
        ResultContainer.Visibility = Visibility.Collapsed;
        ScanButton.IsEnabled = false;
        ScanButton.Content = "SCANNING...";

        // Simulate scan delay
        await Task.Delay(2000);

        // Generate result (70% Nice, 30% Naughty)
        var isNice = Random.Shared.Next(100) < 70;

        // Update result display
        if (isNice)
        {
            ResultIcon.Text = "✨";
            ResultLabel.Text = "NICE LIST";
            ResultLabel.Foreground = (Brush)Application.Current.Resources["ScannerNiceGreenBrush"];
            ResultMessage.Text = NiceMessages[Random.Shared.Next(NiceMessages.Length)];
        }
        else
        {
            ResultIcon.Text = "⚠️";
            ResultLabel.Text = "NAUGHTY LIST";
            ResultLabel.Foreground = (Brush)Application.Current.Resources["ScannerNaughtyRedBrush"];
            ResultMessage.Text = NaughtyMessages[Random.Shared.Next(NaughtyMessages.Length)];
        }

        // Show result
        StatusContainer.Visibility = Visibility.Collapsed;
        ResultContainer.Visibility = Visibility.Visible;

        // Reset button
        ScanButton.IsEnabled = true;
        ScanButton.Content = "SCAN AGAIN";
        IdleText.Text = "Awaiting scan...";
    }
}
