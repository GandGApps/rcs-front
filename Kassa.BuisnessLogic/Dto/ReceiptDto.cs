﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public class ReceiptDto
{
    public Guid Id
    {
        get; set;
    }

    public IngredientUsageDto[] IngredientUsages
    {
        get; set;
    } = [];
}
