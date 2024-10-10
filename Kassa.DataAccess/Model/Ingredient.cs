using System;
using System.Collections.Generic;
using System.Text;
using Kassa.Shared;

namespace Kassa.DataAccess.Model;
public class Ingredient : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public required string Name
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

    public bool IsSellRemainder
    {
        get; set;
    }
}
