using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess.Model;
public record Additive : IModel
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = null!;

    public string CurrencySymbol
    {
        get; set;
    } = null!;

    public double Price
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
    } = null!;

    /// <summary>
    /// Identifier of the product to which this additive belongs.
    /// </summary>
    public Guid[] ProductIds
    {
        get; set;
    }

    /// <summary>
    /// Я забыл зачем это добавил
    /// </summary>
    public int Portion
    {
        get; set;
    }
}
