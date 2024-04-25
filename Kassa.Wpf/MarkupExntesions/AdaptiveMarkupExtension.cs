using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kassa.Wpf.MarkupExntesions;
[MarkupExtensionReturnType(typeof(object))]
public class AdaptiveMarkupExtension : MarkupExtension
{
    public const double LargeBreakpoint = 1300;
    public const double LargeCoeficient = 0.82;

    public const double MediumBreakpoint = 1200;
    public const double MeduimCoeficient = 0.75;

    public const double SmallBreakpoint = 960;
    public const double SmallCoeficient = 0.55;

    public static AdaptiveBreakpoint GetBreakpoint(double width)
    {

        if (width <= SmallBreakpoint)
        {
            return AdaptiveBreakpoint.Small;
        }

        if (width <= MediumBreakpoint)
        {
            return AdaptiveBreakpoint.Medium;
        }

        if (width <= LargeBreakpoint)
        {
            return AdaptiveBreakpoint.Large;
        }


        return AdaptiveBreakpoint.ExtraLarge;
    }

    public static double GetAdaptiveSize(double size, double width)
    {

        if (GetBreakpoint(width) == AdaptiveBreakpoint.Medium)
        {
            return size * MeduimCoeficient;
        }

        if (GetBreakpoint(width) == AdaptiveBreakpoint.Small)
        {
            return size * SmallCoeficient;
        }

        if (GetBreakpoint(width) == AdaptiveBreakpoint.Large)
        {
            return size * LargeCoeficient;
        }

        return size;
    }

    public static double GetNotAdaptivedSize(double size, double width)
    {

        if (GetBreakpoint(width) == AdaptiveBreakpoint.Medium)
        {

            return size / MeduimCoeficient;
        }

        if (GetBreakpoint(width) == AdaptiveBreakpoint.Small)
        {

            return size / SmallCoeficient;
        }

        if (GetBreakpoint(width) == AdaptiveBreakpoint.ExtraLarge)
        {

            return size / LargeCoeficient;
        }

        return size;
    }


    [DefaultValue(AdaptiveBreakpoint.Medium)]
    public AdaptiveBreakpoint Breakpoint
    {
        get; set;
    } = AdaptiveBreakpoint.Medium;

    [DefaultValue(null)]
    public object? Source
    {
        get; set;
    } = null;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var valueProdiver = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

        if (valueProdiver is DependencyObject dependencyObject)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
            {
                var fallbackBinding = new Binding
                {
                    Source = this,
                    Path = new(MainWindow.ActualWidthProperty),
                    Converter = new AdaptiveConverter(),
                    ConverterParameter = Breakpoint,
                    FallbackValue = false
                };

                return fallbackBinding.ProvideValue(serviceProvider);
            }
        }

        var binding = new Binding
        {
            Source = Source ?? MainWindow.Instance,
            Path = new(MainWindow.ActualWidthProperty),
            Converter = new AdaptiveConverter(),
            ConverterParameter = Breakpoint,
            FallbackValue = false
        };

        return binding.ProvideValue(serviceProvider);
    }

    internal class AdaptiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

                var breakpoint = (AdaptiveBreakpoint)parameter;

                if (value is double width)
                {
                    if (width <= 1200 && breakpoint == AdaptiveBreakpoint.Medium)
                    {
                        return true;
                    }

                    if (width > 1200 && breakpoint == AdaptiveBreakpoint.Large)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
