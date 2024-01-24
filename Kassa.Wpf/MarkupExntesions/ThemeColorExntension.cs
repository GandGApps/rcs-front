using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kassa.Wpf.MarkupExntesions;
[MarkupExtensionReturnType(typeof(Color))]
public class ThemeColorExntension : MarkupExtension
{
    public string ColorKey
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var resource = App.Current.Resources[ColorKey];

        if (resource is Color color)
        {
            return color;
        }

        if (resource is SolidColorBrush solidColorBrush)
        {
            return solidColorBrush.Color;
        }

        return Colors.Transparent;
    }
}
