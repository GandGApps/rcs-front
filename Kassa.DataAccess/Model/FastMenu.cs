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
