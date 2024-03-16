using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class Street : IModel
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }

    public Guid DistrictId
    {
        get; set;
    }
}
