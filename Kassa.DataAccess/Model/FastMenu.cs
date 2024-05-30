using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class FastMenu : IModel
{
    public Guid Id
    {
        get; set;
    }

    public Guid CategoryId
    {
        get; set;
    }

    public int Icon
    {
        get; set;
    }

    public int Image
    {
        get; set;
    }
}
