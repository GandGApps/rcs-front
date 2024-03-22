using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class Receipt : IModel
{
    public Guid Id
    {
        get; set;
    }

    public IngredientUsage[] IngredientUsages
    {
        get; set;
    }
}
