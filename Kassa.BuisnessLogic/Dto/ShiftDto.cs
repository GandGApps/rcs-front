using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;

/// <summary>
/// Represent model <see cref="Shift"/> 
/// </summary>
public class ShiftDto : IModel
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

    public Guid? ManagerId
    {
        get; set;
    }

    public Guid[] OrderIds
    {
        get; set;
    } = [];
}
