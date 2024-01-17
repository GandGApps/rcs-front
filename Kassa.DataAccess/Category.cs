namespace Kassa.DataAccess;

public record Category: IModel
{
    public int Id
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

    public int? CategoryId
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
}
