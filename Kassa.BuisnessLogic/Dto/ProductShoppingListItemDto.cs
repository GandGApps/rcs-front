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

    public int ItemId
    {
        get;
    }

    public Guid Id
    {
        get; init;
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
        get; init;
    }
    public string CurrencySymbol
    {
        get;
    }
    public double Discount
    {
        get;
        init;
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
        init;
    }
    public double SubtotalSum => Price * Count;
    public double TotalSum => SubtotalSum * Discount;

    public ICollection<AdditiveShoppingListItemDto> Additives
    {
        get;
    } = [];

    public string? AdditiveInfo
    {
        get; init;
    }
}
