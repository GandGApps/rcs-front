using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
public record FastCategoryDto
{
    public int CategoryId
    {
        get; set;
    }

    public string Icon
    {
        get; set;
    }
}
