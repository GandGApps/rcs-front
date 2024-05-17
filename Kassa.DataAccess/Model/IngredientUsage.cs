using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class IngredientUsage
{

    public Guid IngredientId
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }
}
