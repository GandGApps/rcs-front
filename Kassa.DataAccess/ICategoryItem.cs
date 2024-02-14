using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess;
public interface ICategoryItem: IModel
{
    public Guid? CategoryId
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
