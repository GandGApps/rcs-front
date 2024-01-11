using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess;
public record Product
{
    public int Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = null!;


    public string CurrencySymbol
    {
        get; set;
    } = null!;

    public double Price
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
    } = null!;

    public string Category
    {
        get; set;
    } = null!;

    public string Icon
    {
        get; set;
    } = null!;
}
