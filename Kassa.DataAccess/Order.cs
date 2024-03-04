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

    public DateTime DeliveryTime
    {
        get; set;
    }

    public string Address
    {
        get; set;
    }

    public Courier Courier
    {
        get; set;
    }

    public Guid CourierId
    {
        get; set;
    }

    public IEnumerable<OrderedProduct> Products
    {
        get; set;
    }

    public string Comment
    {
        get; set;
    }

    public int TotalSum
    {
        get; set;
    }

    public Client Client
    {
        get; set;
    }
}
