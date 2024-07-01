using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;

/// <summary>
/// Implement this interface in Presentation Layer to print;
/// </summary>
public interface IPrinter
{
    public Task PrintAsync(ReportShiftDto reportShift);

    public Task PrintAsync(OrderDto order);
}
