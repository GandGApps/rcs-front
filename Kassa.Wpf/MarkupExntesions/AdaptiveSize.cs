using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kassa.Wpf.MarkupExntesions;

[MarkupExtensionReturnType(typeof(Binding))]
public class AdaptiveSizeExtension : MarkupExtension
{
    public double Size
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding
        {
            Source = MainWindow.Instance,
            Path = new("ActualWidth"),
            Converter = new AdaptiveSizeConverter(),
            ConverterParameter = Size
        };

        return binding.ProvideValue(serviceProvider);
    }

    internal class AdaptiveSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = (double)parameter;

            if (value is double width)
            {

                if (width < 1200)
                {
                    return size * 0.69;
                }
            }

            return size;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
