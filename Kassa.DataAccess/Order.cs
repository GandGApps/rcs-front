using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess;
public class Order : IModel
{
    public Guid Id
    {
        get; set;
    }

    public OrderStatus Status
    {
        get; set;
    }

    public DateTime CreatedAt
    {
        get; set;
    }

    public IEnumerable<OrderedProduct> Products
    {
        get;
    }
}
