using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
public class IngredientUsageDto
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
