using Kassa.Shared;

namespace Kassa.DataAccess.Model;

public class OrderedProduct : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public Guid ProductId
    {
        get; set;
    }

    /// <summary>
    /// Сколько заказано
    /// </summary>
    public double Count
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
    public double SubTotalPrice
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }

    /// <summary>
    /// Коментарий к заказу(типо без лука)
    /// </summary>
    public string Comment
    {
        get; set;
    }

    public IEnumerable<OrderedAdditive> Additives
    {
        get; set;
    }
}