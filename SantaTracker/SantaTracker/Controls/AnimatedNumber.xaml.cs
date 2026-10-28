using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace SantaTracker.Controls;

public sealed partial class AnimatedNumber : UserControl
{
    private DispatcherTimer? _timer;
    private double _currentDisplayValue;
    private double _targetValue;
    private double _startValue;
    private int _animationStep;
    private const int TotalSteps = 20; // Number of animation steps
    private const int StepDurationMs = 50; // Duration per step in milliseconds

    public AnimatedNumber()
    {
        this.InitializeComponent();
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(StepDurationMs)
        };
        _timer.Tick += OnTimerTick;

        // Clean up timer on unload (Uno Platform best practice)
        this.Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }
    }

    // Value dependency property
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(AnimatedNumber),
            new PropertyMetadata(0.0, OnValueChanged));

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    // Format string property (e.g., "N0" for integer with commas)
    public static readonly DependencyProperty FormatProperty =
        DependencyProperty.Register(
            nameof(Format),
            typeof(string),
            typeof(AnimatedNumber),
            new PropertyMetadata("N0"));

    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    // TextStyle property to apply a style to the inner TextBlock
    public static readonly DependencyProperty TextStyleProperty =
        DependencyProperty.Register(
            nameof(TextStyle),
            typeof(Style),
            typeof(AnimatedNumber),
            new PropertyMetadata(null, OnTextStyleChanged));

    public Style TextStyle
    {
        get => (Style)GetValue(TextStyleProperty);
        set => SetValue(TextStyleProperty, value);
    }

    // Foreground property
    public new static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register(
            nameof(Foreground),
            typeof(Brush),
            typeof(AnimatedNumber),
            new PropertyMetadata(null, OnForegroundChanged));

    public new Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimatedNumber control)
        {
            control.AnimateToValue((double)e.NewValue);
        }
    }

    private static void OnTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimatedNumber control && e.NewValue is Style style)
        {
            control.NumberText.Style = style;
        }
    }

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimatedNumber control && e.NewValue is Brush brush)
        {
            control.NumberText.Foreground = brush;
        }
    }

    private void AnimateToValue(double newValue)
    {
        _startValue = _currentDisplayValue;
        _targetValue = newValue;
        _animationStep = 0;

        // If the difference is very small, just set directly
        if (Math.Abs(_targetValue - _startValue) < 1)
        {
            _currentDisplayValue = _targetValue;
            UpdateDisplay();
            return;
        }

        // Start animation
        _timer?.Start();
    }

    private void OnTimerTick(object? sender, object e)
    {
        _animationStep++;

        if (_animationStep >= TotalSteps)
        {
            // Animation complete
            _timer?.Stop();
            _currentDisplayValue = _targetValue;
            UpdateDisplay();
            return;
        }

        // Easing function (ease-out quad)
        double progress = (double)_animationStep / TotalSteps;
        double easedProgress = 1 - (1 - progress) * (1 - progress);

        _currentDisplayValue = _startValue + (_targetValue - _startValue) * easedProgress;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        try
        {
            NumberText.Text = _currentDisplayValue.ToString(Format);
        }
        catch
        {
            NumberText.Text = _currentDisplayValue.ToString("N0");
        }
    }
}
