using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Data;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Kassa.Shared;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using static Kassa.Avalonia.MarkupExtensions.AdaptiveMarkupExtension;
using System.Reflection.Metadata;

namespace Kassa.Avalonia.MarkupExtensions;
public sealed class AdaptiveSizeExtension : MarkupExtension
{
    public AdaptiveSizeExtension()
    {
        Size = 0;
    }

    public AdaptiveSizeExtension(double size)
    {
        Size = size;
    }

    [ConstructorArgument("size")]
    [DefaultValue(0)]
    public double Size
    {
        get; set;
    }

    public Thickness Thickness
    {
        get; set;
    }

    public GridLength GridLength
    {
        get; set;
    }

    public CornerRadius CornerRadius
    {
        get; set;
    }

    [DefaultValue(false)]
    public bool IsOnce
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var valueTargetProvider = serviceProvider.GetRequiredService<IProvideValueTarget>();

        var targetObject = valueTargetProvider.TargetObject;
        var targetProperty = valueTargetProvider.TargetProperty;

        if (targetObject is Setter setter)
        {
            targetProperty = setter.Property;
        }

        object value = targetProperty switch
        {
            AvaloniaProperty ap when ap.PropertyType == typeof(Thickness) => Thickness,
            AvaloniaProperty ap when ap.PropertyType == typeof(GridLength) => GridLength,
            AvaloniaProperty ap when ap.PropertyType == typeof(CornerRadius) => CornerRadius,
            _ => Size
        };



#if ONCE_SIZE_ADAPT
        IsOnce = true;
#endif

        if (IsOnce)
        {
            return AdaptiveSizeConverter.AdaptSize(value);
        }

        if (Design.IsDesignMode)
        {
            return value;
        }

        var breakpointPropertyInfo = new ClrPropertyInfo("Breakpoint", x => ((BreakpointNotifier)x).Breakpoint, null, typeof(AdaptiveBreakpoint));
        var builder = new CompiledBindingPathBuilder()
            .Property(breakpointPropertyInfo, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor);

        var binding = new CompiledBindingExtension(builder.Build())
        {
            Mode = BindingMode.OneWay,
            Source = BreakpointNotifier.Instance,
            Converter = AdaptiveSizeConverter.Instance,
            ConverterParameter = value,
            FallbackValue = value
        };

        var providedBinding = binding.ProvideValue(serviceProvider);

        return providedBinding;
    }

    internal sealed class AdaptiveSizeConverter : IValueConverter
    {
        public static AdaptiveSizeConverter Instance = new();

        public static object AdaptSize(object? parameter, AdaptiveBreakpoint? adaptiveBreakpoint = null)
        {
            var breakpoint = adaptiveBreakpoint ?? BreakpointNotifier.Instance.Breakpoint;

            if (parameter is Thickness thickness)
            {
                return new Thickness(
                    GetAdaptiveSize(thickness.Left, breakpoint),
                    GetAdaptiveSize(thickness.Top, breakpoint),
                    GetAdaptiveSize(thickness.Right, breakpoint),
                    GetAdaptiveSize(thickness.Bottom, breakpoint)
                );
            }

            if (parameter is GridLength gridLength)
            {
                if (gridLength.GridUnitType == GridUnitType.Pixel)
                {
                    return new GridLength(
                        GetAdaptiveSize(gridLength.Value, breakpoint),
                        gridLength.GridUnitType
                    );
                }
                return gridLength;
            }

            if (parameter is CornerRadius cornerRadius)
            {
                return new CornerRadius(
                    GetAdaptiveSize(cornerRadius.TopLeft, breakpoint),
                    GetAdaptiveSize(cornerRadius.TopRight, breakpoint),
                    GetAdaptiveSize(cornerRadius.BottomRight, breakpoint),
                    GetAdaptiveSize(cornerRadius.BottomLeft, breakpoint)
                );
            }

            if (parameter is double size)
            {
                return GetAdaptiveSize(size, breakpoint);
            }

            return AvaloniaProperty.UnsetValue;
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not AdaptiveBreakpoint breakpoint)
            {
                return parameter;
            }

            return AdaptSize(parameter, breakpoint);
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
