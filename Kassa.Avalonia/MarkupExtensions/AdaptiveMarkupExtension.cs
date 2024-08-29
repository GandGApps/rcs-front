using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Controls;

namespace Kassa.Avalonia.MarkupExtensions;
public sealed class AdaptiveMarkupExtension : MarkupExtension
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
        return GetAdaptiveSize(size, GetBreakpoint(width));
    }

    public static double GetAdaptiveSize(double size, AdaptiveBreakpoint adaptiveBreakpoint)
    {
        if (adaptiveBreakpoint == AdaptiveBreakpoint.Medium)
        {
            return size * MeduimCoeficient;
        }

        if (adaptiveBreakpoint == AdaptiveBreakpoint.Small)
        {
            return size * SmallCoeficient;
        }

        if (adaptiveBreakpoint == AdaptiveBreakpoint.Large)
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


    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var valueProdiver = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

        if (Design.IsDesignMode)
        {
            return Breakpoint == AdaptiveBreakpoint.ExtraLarge;
        }

        var breakpointPropertyInfo = new ClrPropertyInfo("Breakpoint", x => ((BreakpointNotifier)x).Breakpoint, null, typeof(AdaptiveBreakpoint));
        var builder = new CompiledBindingPathBuilder()
            .Property(breakpointPropertyInfo, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor);

        var binding = new CompiledBindingExtension(builder.Build())
        {
            Mode = BindingMode.OneWay,
            Source = BreakpointNotifier.Instance,
            Converter = AdaptiveConverter.Instance,
            ConverterParameter = Breakpoint,
            FallbackValue = false
        };

        return binding.ProvideValue(serviceProvider);
    }

    internal sealed class AdaptiveConverter : IValueConverter
    {
        public static readonly AdaptiveConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {

                var breakpoint = (AdaptiveBreakpoint?)parameter;

                if (value is double width)
                {
                    return GetBreakpoint(width) == breakpoint;
                }

                if(value is AdaptiveBreakpoint adaptiveBreakpoint)
                {
                    return adaptiveBreakpoint == breakpoint;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}