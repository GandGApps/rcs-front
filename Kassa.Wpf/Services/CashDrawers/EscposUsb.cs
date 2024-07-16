using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Wpf.Services.PosPrinters;
using Splat;

namespace Kassa.Wpf.Services.CashDrawers;
internal sealed class EscposUsb : ICashDrawer, IEnableLogger
{
    public Task Open()
    {
        if (EscPosUsbPrinterContainer.Printer == null)
        {
            this.Log().Warn("No printer found");
            return Task.CompletedTask;
        }

        // Posible concurrency issue
        // Need to use lock with <see cref="EscPosUsbPrinter"/>
        EscPosUsbPrinterContainer.Printer.OpenDrawer();
        EscPosUsbPrinterContainer.Printer.PrintDocument();

        return Task.CompletedTask;
    }
}
