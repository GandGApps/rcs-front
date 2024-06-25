using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class District : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }
}
