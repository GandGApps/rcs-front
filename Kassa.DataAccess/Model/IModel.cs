﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess.Model;
public interface IModel
{
    public Guid Id
    {
        get;
    }
}
