using System;
using System.Collections.Generic;
using System.Text;
using Kassa.Shared;

namespace Kassa.DataAccess.Model;
public class Receipt : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public IngredientUsage[] IngredientUsages
    {
        get; set;
    } = [];
}
