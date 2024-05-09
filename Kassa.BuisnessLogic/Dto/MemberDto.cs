﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public class MemberDto: IModel
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }

    public bool IsAdmin
    {
        get; set;
    }

    public bool IsManager
    {
        get; set;
    }

}