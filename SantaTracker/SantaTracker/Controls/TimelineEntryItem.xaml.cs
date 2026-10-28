using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace SantaTracker.Controls;

public sealed partial class TimelineEntryItem : UserControl
{
    public TimelineEntryItem()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Play entrance animation
        if (Resources.TryGetValue("EntryAnimation", out var resource) && resource is Storyboard storyboard)
        {
            storyboard.Begin();
        }
    }

    #region Dependency Properties

    public static readonly DependencyProperty CityProperty =
        DependencyProperty.Register(nameof(City), typeof(string), typeof(TimelineEntryItem),
            new PropertyMetadata(string.Empty));

    public string City
    {
        get => (string)GetValue(CityProperty);
        set => SetValue(CityProperty, value);
    }

    public static readonly DependencyProperty CountryFlagProperty =
        DependencyProperty.Register(nameof(CountryFlag), typeof(string), typeof(TimelineEntryItem),
            new PropertyMetadata(string.Empty));

    public string CountryFlag
    {
        get => (string)GetValue(CountryFlagProperty);
        set => SetValue(CountryFlagProperty, value);
    }

    public static readonly DependencyProperty ArrivalTimeProperty =
        DependencyProperty.Register(nameof(ArrivalTime), typeof(string), typeof(TimelineEntryItem),
            new PropertyMetadata(string.Empty));

    public string ArrivalTime
    {
        get => (string)GetValue(ArrivalTimeProperty);
        set => SetValue(ArrivalTimeProperty, value);
    }

    public static readonly DependencyProperty FormattedPresentsProperty =
        DependencyProperty.Register(nameof(FormattedPresents), typeof(string), typeof(TimelineEntryItem),
            new PropertyMetadata(string.Empty));

    public string FormattedPresents
    {
        get => (string)GetValue(FormattedPresentsProperty);
        set => SetValue(FormattedPresentsProperty, value);
    }

    public static readonly DependencyProperty IsCurrentProperty =
        DependencyProperty.Register(nameof(IsCurrent), typeof(bool), typeof(TimelineEntryItem),
            new PropertyMetadata(false));

    public bool IsCurrent
    {
        get => (bool)GetValue(IsCurrentProperty);
        set => SetValue(IsCurrentProperty, value);
    }

    public static readonly DependencyProperty IsVisitedProperty =
        DependencyProperty.Register(nameof(IsVisited), typeof(bool), typeof(TimelineEntryItem),
            new PropertyMetadata(false));

    public bool IsVisited
    {
        get => (bool)GetValue(IsVisitedProperty);
        set => SetValue(IsVisitedProperty, value);
    }

    public static readonly DependencyProperty IsNotVisitedProperty =
        DependencyProperty.Register(nameof(IsNotVisited), typeof(bool), typeof(TimelineEntryItem),
            new PropertyMetadata(false));

    public bool IsNotVisited
    {
        get => (bool)GetValue(IsNotVisitedProperty);
        set => SetValue(IsNotVisitedProperty, value);
    }

    public static readonly DependencyProperty ContentOpacityProperty =
        DependencyProperty.Register(nameof(ContentOpacity), typeof(double), typeof(TimelineEntryItem),
            new PropertyMetadata(1.0));

    public double ContentOpacity
    {
        get => (double)GetValue(ContentOpacityProperty);
        set => SetValue(ContentOpacityProperty, value);
    }

    #endregion
}
