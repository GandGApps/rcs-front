using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess;
public record Product : ICategoryItem
{
    public Guid Id
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
    /// <para>
    /// Posible values: 1,2,3
    /// </para>
    /// <para>
    /// If the array is not empty, then it indicates which favorite category the product is in. 
    /// </para>
    /// <para>
    /// There are only 3 favorite categories, this is 1,2,3
    /// </para>
    /// </summary>
    public int[] Favourites
    {
        get; set;
    } = [];

    public string Icon
    {
        get; set;
    } = null!;
    public Guid? CategoryId
    {
        get; set;
    }
}
