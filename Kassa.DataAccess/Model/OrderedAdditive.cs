using Kassa.Shared;

namespace Kassa.DataAccess.Model;

public class OrderedAdditive : IGuidId
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

    /// <summary>
    /// Сумма с учетом скидки
    /// </summary>
    public double TotalPrice
    {
        get; set;
    }

    /// <summary>
    /// Сумма до скидки
    /// </summary>
    public double SubtotalPrice
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }

    public required string Measure
    {
        get; set;
    }
}