using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;

namespace SantaTracker.Controls;

public sealed partial class NorthPoleFM : UserControl
{
    private bool _isPlaying = true;
    private string _currentStation = "classics";
    private int _currentTrackIndex = 0;
    private DispatcherTimer? _visualizerTimer;
    private readonly List<Border> _visualizerBars = new();

    // Playlists
    private readonly Dictionary<string, List<(string Title, string Artist)>> _playlists = new()
    {
        ["classics"] = new()
        {
            ("Jingle Bell Rock", "Bobby Helms"),
            ("Rockin' Around the Christmas Tree", "Brenda Lee"),
            ("It's the Most Wonderful Time", "Andy Williams"),
            ("Let It Snow!", "Dean Martin"),
            ("White Christmas", "Bing Crosby")
        },
        ["carols"] = new()
        {
            ("O Holy Night", "North Pole Choir"),
            ("Silent Night", "Elf Ensemble"),
            ("Away in a Manger", "Reindeer Voices"),
            ("The First Noel", "Arctic Singers"),
            ("Joy to the World", "Santa's Workshop")
        },
        ["jazz"] = new()
        {
            ("Have Yourself a Merry Little Christmas", "Ella Fitzgerald"),
            ("The Christmas Song", "Nat King Cole"),
            ("Winter Wonderland", "Tony Bennett"),
            ("Santa Baby", "Eartha Kitt"),
            ("Blue Christmas", "Elvis Presley")
        },
        ["pop"] = new()
        {
            ("All I Want for Christmas Is You", "Mariah Carey"),
            ("Last Christmas", "Wham!"),
            ("Underneath the Tree", "Kelly Clarkson"),
            ("Santa Tell Me", "Ariana Grande"),
            ("Snowman", "Sia")
        }
    };

    // Station buttons for easy access
    private Button[]? _stationButtons;

    public NorthPoleFM()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _stationButtons = new[] { ClassicsButton, CarolsButton, JazzButton, PopButton };

        // Create visualizer bars
        CreateVisualizerBars();

        // Start animations
        StartAnimations();

        // Start visualizer timer
        _visualizerTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _visualizerTimer.Tick += OnVisualizerTick;
        _visualizerTimer.Start();

        // Set initial track
        UpdateTrackDisplay();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _visualizerTimer?.Stop();
        _visualizerTimer = null;
        StopAnimations();
    }

    private void CreateVisualizerBars()
    {
        VisualizerContainer.Children.Clear();
        _visualizerBars.Clear();

        // Create 12 bars (compact horizontal layout)
        for (int i = 0; i < 12; i++)
        {
            var bar = new Border
            {
                Width = 4,
                Height = Random.Shared.Next(10, 40),
                Background = (Brush)Resources["VisualizerBarGradient"]
                    ?? Application.Current.Resources["VisualizerBarGradient"] as Brush
                    ?? new SolidColorBrush(Microsoft.UI.Colors.Red),
                CornerRadius = new CornerRadius(2),
                VerticalAlignment = VerticalAlignment.Bottom
            };

            _visualizerBars.Add(bar);
            VisualizerContainer.Children.Add(bar);
        }
    }

    private void OnVisualizerTick(object? sender, object e)
    {
        if (!_isPlaying)
            return;

        // Animate bar heights randomly
        foreach (var bar in _visualizerBars)
        {
            bar.Height = Random.Shared.Next(8, 40);
        }
    }

    private void StartAnimations()
    {
        // Start vinyl spin
        if (Resources["VinylSpinAnimation"] is Storyboard vinylSpin)
        {
            vinylSpin.Begin();
        }

        // Start sound waves
        if (Resources["SoundWave1Animation"] is Storyboard wave1)
        {
            wave1.Begin();
        }
        if (Resources["SoundWave2Animation"] is Storyboard wave2)
        {
            wave2.Begin();
        }
        if (Resources["SoundWave3Animation"] is Storyboard wave3)
        {
            wave3.Begin();
        }

        // Start live dot pulse
        if (Resources["LiveDotPulseAnimation"] is Storyboard liveDot)
        {
            liveDot.Begin();
        }
    }

    private void StopAnimations()
    {
        if (Resources["VinylSpinAnimation"] is Storyboard vinylSpin)
        {
            vinylSpin.Stop();
        }
        if (Resources["SoundWave1Animation"] is Storyboard wave1)
        {
            wave1.Stop();
        }
        if (Resources["SoundWave2Animation"] is Storyboard wave2)
        {
            wave2.Stop();
        }
        if (Resources["SoundWave3Animation"] is Storyboard wave3)
        {
            wave3.Stop();
        }
    }

    private void PauseAnimations()
    {
        if (Resources["VinylSpinAnimation"] is Storyboard vinylSpin)
        {
            vinylSpin.Pause();
        }
        if (Resources["SoundWave1Animation"] is Storyboard wave1)
        {
            wave1.Pause();
        }
        if (Resources["SoundWave2Animation"] is Storyboard wave2)
        {
            wave2.Pause();
        }
        if (Resources["SoundWave3Animation"] is Storyboard wave3)
        {
            wave3.Pause();
        }

        // Shrink visualizer bars when paused
        foreach (var bar in _visualizerBars)
        {
            bar.Height = 4;
        }
    }

    private void ResumeAnimations()
    {
        if (Resources["VinylSpinAnimation"] is Storyboard vinylSpin)
        {
            vinylSpin.Resume();
        }
        if (Resources["SoundWave1Animation"] is Storyboard wave1)
        {
            wave1.Resume();
        }
        if (Resources["SoundWave2Animation"] is Storyboard wave2)
        {
            wave2.Resume();
        }
        if (Resources["SoundWave3Animation"] is Storyboard wave3)
        {
            wave3.Resume();
        }
    }

    private void OnPlayPauseClick(object sender, RoutedEventArgs e)
    {
        _isPlaying = !_isPlaying;

        if (_isPlaying)
        {
            PlayPauseIcon.Text = "⏸";
            ResumeAnimations();
        }
        else
        {
            PlayPauseIcon.Text = "▶";
            PauseAnimations();
        }
    }

    private void OnPrevClick(object sender, RoutedEventArgs e)
    {
        var playlist = _playlists[_currentStation];
        _currentTrackIndex--;
        if (_currentTrackIndex < 0)
        {
            _currentTrackIndex = playlist.Count - 1;
        }
        UpdateTrackDisplay();
    }

    private void OnNextClick(object sender, RoutedEventArgs e)
    {
        var playlist = _playlists[_currentStation];
        _currentTrackIndex++;
        if (_currentTrackIndex >= playlist.Count)
        {
            _currentTrackIndex = 0;
        }
        UpdateTrackDisplay();
    }

    private void OnStationClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string station)
        {
            _currentStation = station;
            _currentTrackIndex = 0;
            UpdateTrackDisplay();
            UpdateStationButtons();
        }
    }

    private void UpdateTrackDisplay()
    {
        var playlist = _playlists[_currentStation];
        if (_currentTrackIndex < playlist.Count)
        {
            var track = playlist[_currentTrackIndex];
            SongTitleText.Text = track.Title;
            ArtistNameText.Text = track.Artist;
        }
    }

    private void UpdateStationButtons()
    {
        if (_stationButtons is null)
            return;

        foreach (var button in _stationButtons)
        {
            if (button.Tag is string tag && tag == _currentStation)
            {
                // Active state
                button.Background = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0x33, 0xc4, 0x1e, 0x3a));
                button.BorderBrush = (Brush)Resources["RadioCrimsonBrush"]
                    ?? Application.Current.Resources["RadioCrimsonBrush"] as Brush
                    ?? new SolidColorBrush(Microsoft.UI.Colors.Red);
                button.Foreground = (Brush)Resources["ScannerSnowBrush"]
                    ?? Application.Current.Resources["ScannerSnowBrush"] as Brush
                    ?? new SolidColorBrush(Microsoft.UI.Colors.White);
            }
            else
            {
                // Inactive state
                button.Background = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0x0D, 0xFF, 0xFF, 0xFF));
                button.BorderBrush = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0x1A, 0xFF, 0xFF, 0xFF));
                button.Foreground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(0x99, 0xFF, 0xFF, 0xFF));
            }
        }
    }
}
