using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Kassa.DataAccess.Model;
public class Shift : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public int Number
    {
        get; set;
    }

    public Guid MemberId
    {
        get; set;
    }

    public Guid CashierShiftId
    {
        get; set;
    }

    public DateTime? Start
    {
        get; set;
    }

    public DateTime? End
    {
        get; set;
    }

    public DateTime? BreakStart
    {
        get; set;
    }

    public DateTime? BreakEnd
    {
        get; set;
    }

    public double HourlyRate
    {
        get; set;
    }

    public double Earned
    {
        get; set;
    }

    public double Fine
    {
        get; set;
    }

    public string? Note
    {
        get; set;
    }

    public bool IsStarted
    {
        get; set;
    }

    public Guid ManagerId
    {
        get; set;
    }

    public Guid[] OrderIds
    {
        get; set;
    } = [];
}
