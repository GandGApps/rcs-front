using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Kassa.Wpf.Converters;

public sealed class IntToProjectIcon : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var projectIcon = (int)value;

        var resourceKey = $"ProjectIcon{projectIcon}";

        return Application.Current.FindResource(resourceKey);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public static Geometry? GetProjectIcon(int projectIcon)
    {
        var resourceKey = $"ProjectIcon{projectIcon}";

        return Application.Current.TryFindResource(resourceKey) as Geometry;
    }
}
