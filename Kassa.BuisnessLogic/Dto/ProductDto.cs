using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public record ProductDto : ICategoryItemDto
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

    public string Icon
    {
        get; set;
    } = null!;
    public Guid? CategoryId
    {
        get; set;
    }

    public bool IsAdded
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

    public ICollection<ICategoryItem>? Items
    {
        get;
    } = null;

    public virtual ReceiptDto Receipt
    {
        get; set;
    }

    public Guid ReceiptId
    {
        get; set;
    }

}
