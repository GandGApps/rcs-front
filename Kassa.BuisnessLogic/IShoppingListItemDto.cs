namespace Kassa.BuisnessLogic;

public interface IShoppingListItemDto
{
    public Guid Id
    {
        get; init;
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
        get;
    }
}