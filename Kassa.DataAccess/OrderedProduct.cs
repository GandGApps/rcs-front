
namespace Kassa.DataAccess;

public class OrderedProduct : IModel
{
    public Guid Id
    {
        get; set;
    }

    public Guid ProductId
    {
        get; set;
    }

    public int Count
    {
        get; set;
    }

    public double Price
    {
        get; set;
    }

    public double TotalPrice
    {
        get; set;
    }

    public double SubTotalPrice
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }

    public IEnumerable<OrderedAdditive> Additives
    {
        get; set;
    }
}