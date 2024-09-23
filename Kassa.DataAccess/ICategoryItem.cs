using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.DataAccess;
public interface ICategoryItem: IGuidId
{
    public Guid? CategoryId
    {
        get;
    }

    public string Name
    {
        get;
    }
}
