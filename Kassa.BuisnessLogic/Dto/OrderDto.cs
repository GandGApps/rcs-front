using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public class OrderDto
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
        get; set;
    }
}