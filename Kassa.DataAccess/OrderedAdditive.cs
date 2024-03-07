﻿
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

    public string Measure
    {
        get; set;
    }
}