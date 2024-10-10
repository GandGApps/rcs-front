namespace Kassa.DataAccess.Model;

public record Category : ICategoryItem
{
    public Guid Id
    {
        get; set;
    }

    public required string Name
    {
        get; set;
    }

    public int Image
    {
        get; set;
    }

    public required string Color
    {
        get; set;
    }

    public Guid? CategoryId
    {
        get; set;
    }

    public  virtual required ICollection<Category> Categories
    {
        get; set;
    }

    public virtual required ICollection<Product> Products
    {
        get; set;
    }

    public ICollection<ICategoryItem>? Items
    {
        get;
    }
}
