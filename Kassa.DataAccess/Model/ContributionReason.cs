﻿using System;
using System.Collections.Generic;
using System.Text;
using Kassa.Shared;

namespace Kassa.DataAccess.Model;
public class ContributionReason : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public required string Name
    {
        get; set;
    }

    public bool IsRequiredComment
    {
        get; set;
    }
}
