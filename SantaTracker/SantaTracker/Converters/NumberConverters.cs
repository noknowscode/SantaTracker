using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace SantaTracker.Converters;

/// <summary>
/// Formats large numbers with commas (e.g., 421,405,244)
/// </summary>
public class NumberFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is long longValue)
        {
            return longValue.ToString("N0");
        }
        if (value is int intValue)
        {
            return intValue.ToString("N0");
        }
        if (value is double doubleValue)
        {
            return doubleValue.ToString("N0");
        }
        return value?.ToString() ?? "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Formats visits count (e.g., "1,234,567" visits)
/// </summary>
public class VisitsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is long longValue)
        {
            return longValue.ToString("N0");
        }
        if (value is int intValue)
        {
            return intValue.ToString("N0");
        }
        return value?.ToString() ?? "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Formats distance with K suffix (e.g., "12,345K")
/// </summary>
public class DistanceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            var km = doubleValue / 1000;
            return $"{km:N0}K";
        }
        if (value is long longValue)
        {
            var km = longValue / 1000;
            return $"{km:N0}K";
        }
        return value?.ToString() ?? "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Returns visits count as a double (toys / 100 = approx families visited)
/// Used with AnimatedNumber control
/// </summary>
public class VisitsMultiplierConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is long longValue)
        {
            return (double)(longValue / 100); // Approx families visited
        }
        if (value is int intValue)
        {
            return (double)(intValue / 100);
        }
        if (value is double doubleValue)
        {
            return doubleValue / 100;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts IsNice boolean to appropriate color brush (green for nice, red for naughty)
/// </summary>
public class NiceToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isNice)
        {
            return isNice
                ? (Brush)Application.Current.Resources["CheerNiceBrush"]
                : (Brush)Application.Current.Resources["CheerNaughtyBrush"];
        }
        return (Brush)Application.Current.Resources["TextOnCreamBrush"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
