using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
public class OrderedAdditiveDto
{
    public Guid Id
    {
        get; set;
    }

    public Guid AdditiveId
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

    public double SubtotalPrice
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }

    public string Measure
    {
        get; set;
    }
}
