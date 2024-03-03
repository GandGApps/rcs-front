
namespace Kassa.DataAccess;

public class OrderedAdditive : IModel
{
    public Guid Id
    {
        get; set;
    }

    public Guid AdditiveId
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

    public double SubtotalPrice
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }
}