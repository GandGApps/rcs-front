using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kassa.Wpf.Converters;
public class IsValueLessThanParameter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value is not double doubleValue)
        {

            return false;
        }

        if (parameter is not double doubleParameter)
        {

            return false;
        }

        return doubleValue < doubleParameter;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
