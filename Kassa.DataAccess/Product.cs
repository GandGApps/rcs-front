using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess;
public record Product : ICategoryItem
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

    /// <summary>
    /// Need to be removed, as soon as possible
    /// </summary>
    [Obsolete]
    public string[] Categories
    {
        get; set;
    } = [];


    public string Icon
    {
        get; set;
    } = null!;
    public int CategoryId
    {
        get; set;
    }
    public ICollection<ICategoryItem>? Items => null;
}
