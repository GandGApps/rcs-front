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

[MarkupExtensionReturnType(typeof(Binding))]
public class AdaptiveSizeExtension : MarkupExtension
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

    public override object ProvideValue(IServiceProvider serviceProvider)
    {

        var targetProperty = serviceProvider?.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target ? target.TargetProperty : null;
        Binding binding;

        if (targetProperty is DependencyProperty property)
        {
            if (property.PropertyType == typeof(Thickness))
            {
                binding = new Binding
                {
                    Source = MainWindow.Instance,
                    Path = new("ActualWidth"),
                    Converter = new AdaptiveSizeConverter(),
                    ConverterParameter = Thickness,
                    FallbackValue = Thickness
                };

                return binding.ProvideValue(serviceProvider);
            }
        }

        binding = new Binding
        {
            Source = MainWindow.Instance,
            Path = new("ActualWidth"),
            Converter = new AdaptiveSizeConverter(),
            ConverterParameter = Size,
            FallbackValue = Size
        };

        return binding.ProvideValue(serviceProvider);
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
            var size = (double)parameter;

            return AdaptiveMarkupExtension.GetAdaptiveSize(size, width);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
