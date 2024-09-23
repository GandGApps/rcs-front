using System;
using System.Collections.Generic;
using System.Text;
using Kassa.Shared;

namespace Kassa.DataAccess.Model;
public class FastMenu : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public Guid CategoryId
    {
        get; set;
    }

    public string ImageSource
    {
        get; set;
    }

    public int Image
    {
        get; set;
    }

    public string Color
    {
        get; set;
    }
}
