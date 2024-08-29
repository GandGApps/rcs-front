using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kassa.Wpf.MarkupExntesions;

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

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if(serviceProvider == null) 
        {
            var fallbackBinding = new Binding
            {
                Source = GetSource(serviceProvider),
                Path = new(FrameworkElement.ActualWidthProperty),
                Converter = new AdaptiveSizeConverter(),
                ConverterParameter = Size,
                FallbackValue = Size
            };

            return fallbackBinding.ProvideValue(serviceProvider);
        }

        var valueTargetProvider = serviceProvider?.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

        var targetProperty = valueTargetProvider?.TargetProperty;
        var targetObject = valueTargetProvider?.TargetObject;


        if (targetObject is DependencyObject dependencyObject)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
            {
                if (targetProperty is DependencyProperty dProperty)
                {
                    if (dProperty.PropertyType == typeof(Thickness))
                    {
                        return Thickness;
                    }
                    if (dProperty.PropertyType == typeof(GridLength))
                    {
                        return GridLength;
                    }
                    if (dProperty.PropertyType == typeof(CornerRadius))
                    {
                        return CornerRadius;
                    }
                }
                return Size;
            }
        }

        if (MainWindow.Instance == null)
        {
            if (targetObject is Setter dSetter)
            {
                if (dSetter.Property.PropertyType == typeof(Thickness))
                {
                    return Thickness;
                }

                if (dSetter.Property.PropertyType == typeof(GridLength))
                {
                    return GridLength;
                }

                if (dSetter.Property.PropertyType == typeof(CornerRadius))
                {
                    return CornerRadius;
                }

                return Size;
            }
        }

        Binding binding;
        var source = GetSource(serviceProvider);

        if (targetObject is Setter setter)
        {
            if (setter.Property is null)
            {
                return null!;
            }
            if (setter.Property.PropertyType == typeof(Thickness))
            {
                binding = new Binding
                {
                    Source = source,
                    Path = new(FrameworkElement.ActualWidthProperty),
                    Converter = new AdaptiveSizeConverter(),
                    ConverterParameter = Thickness,
                    FallbackValue = Thickness
                };

                return binding.ProvideValue(serviceProvider);
            }

            if (setter.Property.PropertyType == typeof(GridLength))
            {
                binding = new Binding
                {
                    Source = source,
                    Path = new(FrameworkElement.ActualWidthProperty),
                    Converter = new AdaptiveSizeConverter(),
                    ConverterParameter = GridLength,
                    FallbackValue = GridLength
                };

                return binding.ProvideValue(serviceProvider);
            }

            if (setter.Property.PropertyType == typeof(CornerRadius))
            {
                binding = new Binding
                {
                    Source = source,
                    Path = new(FrameworkElement.ActualWidthProperty),
                    Converter = new AdaptiveSizeConverter(),
                    ConverterParameter = CornerRadius,
                    FallbackValue = CornerRadius
                };

                return binding.ProvideValue(serviceProvider);
            }
        }

        if (targetProperty is DependencyProperty property)
        {
            if (property.PropertyType == typeof(Thickness))
            {
                binding = new Binding
                {
                    Source = source,
                    Path = new(FrameworkElement.ActualWidthProperty),
                    Converter = new AdaptiveSizeConverter(),
                    ConverterParameter = Thickness,
                    FallbackValue = Thickness
                };

                return binding.ProvideValue(serviceProvider);
            }

            if (property.PropertyType == typeof(GridLength))
            {
                binding = new Binding
                {
                    Source = source,
                    Path = new(FrameworkElement.ActualWidthProperty),
                    Converter = new AdaptiveSizeConverter(),
                    ConverterParameter = GridLength,
                    FallbackValue = GridLength
                };

                return binding.ProvideValue(serviceProvider);
            }

            if (property.PropertyType == typeof(CornerRadius))
            {

                binding = new Binding
                {
                    Source = source,
                    Path = new(FrameworkElement.ActualWidthProperty),
                    Converter = new AdaptiveSizeConverter(),
                    ConverterParameter = CornerRadius,
                    FallbackValue = CornerRadius
                };

                return binding.ProvideValue(serviceProvider);
            }
        }



        binding = new Binding
        {
            Source = source,
            Path = new(FrameworkElement.ActualWidthProperty),
            Converter = new AdaptiveSizeConverter(),
            ConverterParameter = Size,
            FallbackValue = Size
        };

        return binding.ProvideValue(serviceProvider);
    }

    private static object GetSource(IServiceProvider? serviceProvider)
    {

        var designerHost = serviceProvider?.GetService(typeof(IDesignerHost)) as IDesignerHost;

        if (designerHost is not null)
        {
            return designerHost.RootComponent;
        }

        if (MainWindow.Instance is not null)
        {
            return MainWindow.Instance;
        }

        return null!;
    }

    internal class AdaptiveSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double width)
            {
                return parameter;
            }

            if (parameter is Thickness thickness)
            {
                return new Thickness(
                    AdaptiveMarkupExtension.GetAdaptiveSize(thickness.Left, width),
                    AdaptiveMarkupExtension.GetAdaptiveSize(thickness.Top, width),
                    AdaptiveMarkupExtension.GetAdaptiveSize(thickness.Right, width),
                    AdaptiveMarkupExtension.GetAdaptiveSize(thickness.Bottom, width)
                );
            }

            if (parameter is GridLength gridLength)
            {
                if (gridLength.GridUnitType == GridUnitType.Pixel)
                {
                    return new GridLength(
                        AdaptiveMarkupExtension.GetAdaptiveSize(gridLength.Value, width),
                        gridLength.GridUnitType
                    );
                }
                return gridLength;
            }

            if (parameter is CornerRadius cornerRadius)
            {
                return new CornerRadius(
                    AdaptiveMarkupExtension.GetAdaptiveSize(cornerRadius.TopLeft, width),
                    AdaptiveMarkupExtension.GetAdaptiveSize(cornerRadius.TopRight, width),
                    AdaptiveMarkupExtension.GetAdaptiveSize(cornerRadius.BottomRight, width),
                    AdaptiveMarkupExtension.GetAdaptiveSize(cornerRadius.BottomLeft, width)
                );
            }

            var size = (double)parameter;

            return AdaptiveMarkupExtension.GetAdaptiveSize(size, width);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
