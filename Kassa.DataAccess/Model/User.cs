﻿using System;
using System.Collections.Generic;
using System.Text;
using Kassa.Shared;

namespace Kassa.DataAccess.Model;
public class User : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string? Name
    {
        get; set;
    }
}
