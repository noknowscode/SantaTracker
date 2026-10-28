using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;
using Color = Mapsui.Styles.Color;

namespace SantaTracker.Controls;

/// <summary>
/// A map control using MapsUI with coordinate display and Santa marker
/// </summary>
public sealed partial class SantaMapView : UserControl
{
    private MemoryLayer? _markerLayer;
    private bool _isMapInitialized;

    public SantaMapView()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
    }

    #region Dependency Properties

    public static readonly DependencyProperty LatitudeProperty =
        DependencyProperty.Register(nameof(Latitude), typeof(double), typeof(SantaMapView),
            new PropertyMetadata(0.0, OnCoordinateChanged));

    public static readonly DependencyProperty LongitudeProperty =
        DependencyProperty.Register(nameof(Longitude), typeof(double), typeof(SantaMapView),
            new PropertyMetadata(0.0, OnCoordinateChanged));

    public double Latitude
    {
        get => (double)GetValue(LatitudeProperty);
        set => SetValue(LatitudeProperty, value);
    }

    public double Longitude
    {
        get => (double)GetValue(LongitudeProperty);
        set => SetValue(LongitudeProperty, value);
    }

    public string CoordinatesDisplay
    {
        get
        {
            var latDir = Latitude >= 0 ? "N" : "S";
            var lonDir = Longitude >= 0 ? "E" : "W";
            return $"LAT {Math.Abs(Latitude):F2}°{latDir}  LON {Math.Abs(Longitude):F2}°{lonDir}";
        }
    }

    private static void OnCoordinateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SantaMapView mapView)
        {
            mapView.Bindings.Update();
            mapView.UpdateMarker();
        }
    }

    #endregion

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeMap();
    }

    private void InitializeMap()
    {
        var map = MapControl.Map;

        // Add OpenStreetMap tile layer
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        // Create marker layer
        _markerLayer = new MemoryLayer { Name = "SantaMarker" };
        map.Layers.Add(_markerLayer);

        // Set initial view (world centered)
        var (x, y) = SphericalMercator.FromLonLat(0, 20);
        var centerPoint = new MPoint(x, y);
        map.Navigator.CenterOnAndZoomTo(centerPoint, map.Navigator.Resolutions[2]);

        // Dark background
        map.BackColor = Color.FromArgb(255, 26, 42, 35);

        _isMapInitialized = true;
        UpdateMarker();
    }

    private void UpdateMarker()
    {
        if (!_isMapInitialized || _markerLayer is null)
            return;

        // Convert lat/lon to map coordinates
        var (x, y) = SphericalMercator.FromLonLat(Longitude, Latitude);

        // Pan map to follow the marker
        MapControl.Map.Navigator.CenterOn(new MPoint(x, y));

        // Create marker feature with pulsing red dot style
        var point = new Point(x, y);
        var feature = new GeometryFeature(point);

        // Outer glow circle
        feature.Styles.Add(new SymbolStyle
        {
            Fill = new Mapsui.Styles.Brush(Color.FromArgb(100, 196, 30, 58)),
            SymbolScale = 1.2,
            SymbolType = SymbolType.Ellipse
        });

        // Inner solid circle
        feature.Styles.Add(new SymbolStyle
        {
            Fill = new Mapsui.Styles.Brush(Color.FromArgb(255, 196, 30, 58)),
            Outline = new Pen(Color.White, 2),
            SymbolScale = 0.6,
            SymbolType = SymbolType.Ellipse
        });

        _markerLayer.Features = [feature];
        _markerLayer.DataHasChanged();
    }
}
