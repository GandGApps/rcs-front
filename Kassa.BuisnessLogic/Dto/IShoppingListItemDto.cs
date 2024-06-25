using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;

public interface IShoppingListItemDto: IGuidId
{
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
        get;
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
        get; set;
    }
    public double SubtotalSum
    {
        get;
    }
    public double TotalSum
    {
        get;
    }
}