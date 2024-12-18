﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public class FastMenuDto : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public Guid CategoryId
    {
        get; set;
    }

    public string? ImageSource
    {
        get; set;
    }

    public int Image
    {
        get; set;
    }

    public string? Color
    {
        get; set;
    }
}
