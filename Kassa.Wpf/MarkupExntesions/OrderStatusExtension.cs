using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Kassa.DataAccess.Model;

namespace Kassa.Wpf.MarkupExntesions;
public class OrderStatusExtension : MarkupExtension
{
    public OrderStatus OrderStatus
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return OrderStatus;
    }
}
