using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;

namespace Kassa.Wpf.Services;
internal sealed class EcsPrinter : IPrinter
{
    public async Task PrintAsync(ReportShiftDto reportShift)
    {
        Console.WriteLine("Printing report shift");
    }

    public async Task PrintAsync(OrderDto order)
    {
        Console.WriteLine("Printing order");
    }
}
