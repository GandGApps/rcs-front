namespace Kassa.BuisnessLogic.Dto;

public interface IShoppingListItemDto
{
    public Guid Id
    {
        get; init;
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
        get;
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
        get; init;
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