using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public interface ICategoryItemDto
{
    public int? CategoryId
    {
        get;
    }

    public string Name
    {
        get;
    }

    public ICollection<ICategoryItem>? Items
    {
        get;
    }
}
