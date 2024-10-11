using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess.Model;
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


    public int Image
    {
        get; set;
    }

    public Guid? CategoryId
    {
        get; set;
    }

    public Guid ReceiptId
    {
        get; set;
    }

    public bool IsAvailable
    {
        get; set;
    } = true;

    public bool IsEnoughIngredients
    {
        get; set;
    } = true;

    public string? Color
    {
        get; set;
    } 
}
