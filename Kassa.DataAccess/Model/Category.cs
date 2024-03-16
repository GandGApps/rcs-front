namespace Kassa.DataAccess.Model;

public record Category : ICategoryItem
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }

    public string? Icon
    {
        get; set;
    }

    public Guid? CategoryId
    {
        get; set;
    }

    public virtual ICollection<Category> Categories
    {
        get; set;
    }

    public virtual ICollection<Product> Products
    {
        get; set;
    }

    public ICollection<ICategoryItem>? Items
    {
        get;
    }
}
