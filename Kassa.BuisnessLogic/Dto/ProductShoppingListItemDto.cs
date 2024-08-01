using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public record ProductShoppingListItemDto : IShoppingListItemDto
{

    public ProductShoppingListItemDto(ProductDto product)
    {
        ItemId = product.Id;
        Name = product.Name;
        Price = product.Price;
        CurrencySymbol = product.CurrencySymbol;
        Measure = product.Measure;
        Count = 1;
        Discount = 1;
    }

    public ProductShoppingListItemDto(OrderedProductDto orderedProduct, ProductDto product)
    {
        Id = orderedProduct.Id;
        ItemId = orderedProduct.ProductId;
        Name = product.Name;
        Price = orderedProduct.Price;
        CurrencySymbol = product.CurrencySymbol;
        Measure = product.Measure;
        Count = orderedProduct.Count;
        Discount = orderedProduct.Discount;
        HasDiscount = orderedProduct.Discount < 1;
        Comment = orderedProduct.Comment;
    }

    /// <summary>
    /// Product id
    /// </summary>
    public Guid ItemId
    {
        get;
    }

    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get;
    }

    public bool IsSelected
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }
    public string CurrencySymbol
    {
        get;
    }
    public double Discount
    {
        get;
        set;
    }
    public bool HasDiscount
    {
        get;
        set;
    }
    public string Measure
    {
        get;
    }
    public double Price
    {
        get;
        set;
    }

    public List<AdditiveShoppingListItemDto> Additives
    {
        get;
    } = [];

    public double SubtotalSum
    {
        get; set;
    }
    public double TotalSum => SubtotalSum * Discount;

    public string? Comment
    {
        get; set;
    }
}
