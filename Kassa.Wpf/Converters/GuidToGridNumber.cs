using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kassa.Wpf.Converters;
public class GuidToGridNumber : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var take = (int?)parameter ?? 5;
        if (value is Guid guid)
        {
            var firstFiveChars = guid.ToString("N")[..take];
            return firstFiveChars;
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
