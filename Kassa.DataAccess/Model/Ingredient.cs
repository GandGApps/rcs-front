﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class Ingredient : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }

    public string Measure
    {
        get; set;
    }

    public bool IsSellRemainder
    {
        get; set;
    }
}