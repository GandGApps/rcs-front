using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess;
public class Additive : IModel
{
    public int Id
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
    public int[] ProductIds
    {
        get; set;
    }
}
