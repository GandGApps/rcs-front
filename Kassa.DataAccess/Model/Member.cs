using System;
using System.Collections.Generic;
using System.Text;
using Kassa.Shared;

namespace Kassa.DataAccess.Model;
public class Member : IGuidId
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

    // TODO: Unuse this property, it's mocked
    internal string Pincode
    {
        get; set;
    }
}
