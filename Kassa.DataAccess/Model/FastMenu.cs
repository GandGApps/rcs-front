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

    public required string ImageSource
    {
        get; set;
    }

    public int Image
    {
        get; set;
    }

    public required string Color
    {
        get; set;
    }
}
