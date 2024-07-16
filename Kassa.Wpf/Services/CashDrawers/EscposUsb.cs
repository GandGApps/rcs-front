using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
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

        EscPosUsbPrinterContainer.Printer.OpenDrawer();

        return Task.CompletedTask;
    }
}
