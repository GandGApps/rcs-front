using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public record AdditiveShoppingListItemDto : IShoppingListItemDto, IModel
{

    public AdditiveShoppingListItemDto(ProductShoppingListItemDto productShopping,AdditiveDto additiveDto)
    {
        ItemId = additiveDto.Id;
        Name = additiveDto.Name;
        Price = additiveDto.Price;
        CurrencySymbol = additiveDto.CurrencySymbol;
        Measure = additiveDto.Measure;
        Count = 1;
        Discount = 1;
        Portion = additiveDto.Portion;
        TotalSum = Price;
        SubtotalSum = Price;

        IsSelected = false;
        ContainingProduct = productShopping;
    }

    public AdditiveShoppingListItemDto(OrderedAdditiveDto orderedAdditive, ProductShoppingListItemDto productShopping, AdditiveDto additiveDto)
    {
        Id = orderedAdditive.Id;
        ItemId = additiveDto.Id;
        Name = additiveDto.Name;
        Price = additiveDto.Price;
        CurrencySymbol = additiveDto.CurrencySymbol;
        Measure = additiveDto.Measure;
        Count = orderedAdditive.Count;
        Discount = orderedAdditive.Discount;
        Portion = additiveDto.Portion;
        SubtotalSum = orderedAdditive.SubtotalPrice;
        TotalSum = orderedAdditive.TotalPrice;

        IsSelected = true;
        ContainingProduct = productShopping;
    }

    public Guid Id
    {
        get;
        set;
    }
    public Guid ItemId
    {
        get;
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
        get;
        set;
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
    public double SubtotalSum
    {
        get;
    }
    public double TotalSum
    {
        get;
    }

    public int Portion
    {
        get;
    }

    public ProductShoppingListItemDto ContainingProduct
    {
        get;
    } 
}
