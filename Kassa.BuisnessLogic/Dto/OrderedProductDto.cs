using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public class OrderedProductDto
{
    public Guid Id
    {
        get; set;
    }

    public Guid ProductId
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }

    public double Price
    {
        get; set;
    }

    public double TotalPrice
    {
        get; set;
    }

    public double SubTotalPrice
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }

    public string? Comment
    {
        get; set;
    }

    public IEnumerable<OrderedAdditiveDto> Additives
    {
        get; set;
    } = [];
}
