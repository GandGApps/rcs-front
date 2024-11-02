using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kassa.Wpf.MarkupExntesions;

public sealed class AdaptiveSizeExtension : MarkupExtension
{
    private static readonly AdaptiveSizeConverter _adaptiveSizeConverter = new();
    private static readonly PropertyPath _actualWidthPropertyPath = new(FrameworkElement.ActualWidthProperty);

#if DEBUG
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
    private static int _counter = 0;
#endif

    public AdaptiveSizeExtension()
    {
#if DEBUG

        _counter++;
#endif
    }

    public AdaptiveSizeExtension(double size): this()
    {
        Size = size;
    }

    [ConstructorArgument("size")]
    public double? Size
    {
        get; set;
    }

    public Thickness? Thickness
    {
        get; set;
    }

    public GridLength? GridLength
    {
        get; set;
    }

    public CornerRadius? CornerRadius
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
                Path = _actualWidthPropertyPath,
                Converter = _adaptiveSizeConverter,
                ConverterParameter = Size,
                FallbackValue = Size
            };

            return fallbackBinding.ProvideValue(serviceProvider);
        }

        var valueTargetProvider = serviceProvider?.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

        var targetProperty = valueTargetProvider?.TargetProperty;
        var targetObject = valueTargetProvider?.TargetObject;


        if (targetObject is DependencyObject)
        {
            if (IsDesignMode(serviceProvider, targetObject, targetProperty))
            {
                if (targetProperty is DependencyProperty dProperty)
                {
                    if (dProperty.PropertyType == typeof(Thickness))
                    {
                        return Thickness ?? new();
                    }
                    if (dProperty.PropertyType == typeof(GridLength))
                    {
                        return GridLength ?? new();
                    }
                    if (dProperty.PropertyType == typeof(CornerRadius))
                    {
                        return CornerRadius ?? new();
                    }
                }
                return Size ?? 1;
            }
        }
        
        Binding binding;
        var source = GetSource(serviceProvider);

        if (targetObject is Setter setter)
        {
            if (IsDesignMode(serviceProvider, targetObject, targetProperty))
            {
                return ReturnFirstNotNullProperty();
            }
            if (setter.Property.PropertyType == typeof(Thickness))
            {
                binding = new Binding
                {
                    Source = source,
                    Path = _actualWidthPropertyPath,
                    Converter = _adaptiveSizeConverter,
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
                    Path = _actualWidthPropertyPath,
                    Converter = _adaptiveSizeConverter,
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
                    Path = _actualWidthPropertyPath,
                    Converter = _adaptiveSizeConverter,
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
                    Path = _actualWidthPropertyPath,
                    Converter = _adaptiveSizeConverter,
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
                    Path = _actualWidthPropertyPath,
                    Converter = _adaptiveSizeConverter,
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
                    Path = _actualWidthPropertyPath,
                    Converter = _adaptiveSizeConverter,
                    ConverterParameter = CornerRadius,
                    FallbackValue = CornerRadius
                };

                return binding.ProvideValue(serviceProvider);
            }
        }



        binding = new Binding
        {
            Source = source,
            Path = _actualWidthPropertyPath,
            Converter = _adaptiveSizeConverter,
            ConverterParameter = ReturnFirstNotNullProperty(),
            FallbackValue = ReturnFirstNotNullProperty()
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

    private  object ReturnFirstNotNullProperty()
    {
        if (Size is double size)
        {
            return size;
        }

        if(Thickness is Thickness thickness)
        {
            return thickness;
        }

        if (GridLength is GridLength gridLength)
        {
            return gridLength;
        }

        if (CornerRadius is CornerRadius cornerRadius)
        {
            return cornerRadius;
        }

        return 1;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    private static bool IsDesignMode(IServiceProvider? serviceProvider, object? targetObject, object? targetProperty)
    {
        var designerHost = serviceProvider?.GetService(typeof(IDesignerHost)) as IDesignerHost;

        if (designerHost is not null)
        {
            return true;
        }

        if (targetObject is DependencyObject dependencyObject)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
            {
                return true;
            }
        }

        // Очень спорное утверждение, но пока так
        if (MainWindow.Instance is null)
        {
            return true;
        }

        return false;
    }

    internal sealed class AdaptiveSizeConverter : IValueConverter
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
