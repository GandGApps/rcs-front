using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
public record AdditiveShoppingListItemDto : IShoppingListItemDto
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

        IsSelected = false;
        ContainingProduct = productShopping;
    }

    public Guid Id
    {
        get;
        init;
    }
    public int ItemId
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
        init;
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
