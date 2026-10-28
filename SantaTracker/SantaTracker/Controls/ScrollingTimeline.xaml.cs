using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using SantaTracker.Models;
using System.Collections.Immutable;

namespace SantaTracker.Controls;

public sealed partial class ScrollingTimeline : UserControl
{
    private DispatcherTimer? _scrollTimer;
    private DispatcherTimer? _animationTimer;
    private readonly List<MissionLogEntry> _allDestinations = new();
    private readonly List<FrameworkElement> _visibleItems = new();
    private int _currentStartIndex = 0;
    private const int MaxVisibleItems = 5;
    private const double ItemHeight = 44; // Height of each item
    private double _currentScrollOffset = 0;
    private bool _isAnimating = false;

    public ScrollingTimeline()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(object),
            typeof(ScrollingTimeline),
            new PropertyMetadata(null, OnItemsSourceChanged));

    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollingTimeline control)
        {
            control.UpdateItems(e.NewValue);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Start the scroll timer (scroll every 3 seconds)
        _scrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3)
        };
        _scrollTimer.Tick += OnScrollTimerTick;
        _scrollTimer.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _scrollTimer?.Stop();
        _animationTimer?.Stop();
    }

    private void UpdateItems(object? newItems)
    {
        _allDestinations.Clear();

        if (newItems is IImmutableList<MissionLogEntry> immutableList)
        {
            _allDestinations.AddRange(immutableList);
        }
        else if (newItems is IEnumerable<MissionLogEntry> enumerable)
        {
            _allDestinations.AddRange(enumerable);
        }

        // Only rebuild if not currently animating
        if (!_isAnimating)
        {
            RebuildVisibleItems();
        }
    }

    private void RebuildVisibleItems()
    {
        ItemsPanel.Children.Clear();
        _visibleItems.Clear();

        if (_allDestinations.Count == 0) return;

        // Show up to MaxVisibleItems, cycling through the list
        for (int i = 0; i < Math.Min(MaxVisibleItems, _allDestinations.Count + 2); i++)
        {
            var index = (_currentStartIndex + i) % _allDestinations.Count;
            var item = CreateTimelineItem(_allDestinations[index], i == 0);
            ItemsPanel.Children.Add(item);
            _visibleItems.Add(item);
        }

        // Reset scroll transform
        ScrollTransform.Y = 0;
        _currentScrollOffset = 0;
    }

    private FrameworkElement CreateTimelineItem(MissionLogEntry entry, bool isFirst)
    {
        var container = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            Padding = new Thickness(0, 12, 0, 12),
            Height = ItemHeight,
            Opacity = isFirst ? 0.6 : 1.0
        };

        // Pin marker
        var pinGrid = new Grid { Width = 20, Height = 20 };
        var ellipse = new Ellipse
        {
            Width = 20,
            Height = 20,
            Fill = (Brush)Application.Current.Resources["AccentRedBrush"],
            Stroke = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            StrokeThickness = 2
        };
        pinGrid.Children.Add(ellipse);
        container.Children.Add(pinGrid);

        // Text container
        var textPanel = new StackPanel { Spacing = 2 };

        var cityText = new TextBlock
        {
            Text = entry.City,
            Style = (Style)Application.Current.Resources["DestinationCityStyle"]
        };
        textPanel.Children.Add(cityText);

        var coordsText = new TextBlock
        {
            Text = entry.Coordinates,
            Style = (Style)Application.Current.Resources["DestinationCoordsStyle"]
        };
        textPanel.Children.Add(coordsText);

        container.Children.Add(textPanel);

        return container;
    }

    private void OnScrollTimerTick(object? sender, object e)
    {
        if (_allDestinations.Count <= 1 || _isAnimating) return;

        _isAnimating = true;

        // Animate scroll up
        var animationSteps = 15;
        var stepDuration = 30; // ms per step
        var targetOffset = -ItemHeight;
        var stepOffset = targetOffset / animationSteps;
        var currentStep = 0;

        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(stepDuration)
        };

        _animationTimer.Tick += (s, args) =>
        {
            currentStep++;
            _currentScrollOffset += stepOffset;
            ScrollTransform.Y = _currentScrollOffset;

            // Fade out the first item
            if (_visibleItems.Count > 0 && _visibleItems[0] != null)
            {
                var fadeProgress = (double)currentStep / animationSteps;
                _visibleItems[0].Opacity = Math.Max(0, 0.6 - (fadeProgress * 0.6));
            }

            if (currentStep >= animationSteps)
            {
                _animationTimer?.Stop();

                // Move to next item
                _currentStartIndex = (_currentStartIndex + 1) % _allDestinations.Count;

                // Rebuild the list
                RebuildVisibleItems();

                _isAnimating = false;
            }
        };

        _animationTimer.Start();
    }
}
