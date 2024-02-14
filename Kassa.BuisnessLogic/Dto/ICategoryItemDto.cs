using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public interface ICategoryItemDto
{
    public Guid? CategoryId
    {
        get;
    }

    public int[] Favourites
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
