using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kassa.Wpf.MarkupExntesions;

[MarkupExtensionReturnType(typeof(Binding))]
public class AdaptiveMarkupExtension : MarkupExtension
{

    [DefaultValue(AdaptiveBreakpoint.Medium)]
    public AdaptiveBreakpoint Breakpoint
    {
        get; set;
    }

    [DefaultValue(null)]
    public object Source
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding
        {
            Source = Source ?? MainWindow.Instance,
            Path = new(MainWindow.ActualWidthProperty),
            Converter = new AdaptiveConverter(),
            ConverterParameter = Breakpoint
        };

        return binding;
    }

    internal class AdaptiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
