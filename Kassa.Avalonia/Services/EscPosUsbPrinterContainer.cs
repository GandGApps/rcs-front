using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESC_POS_USB_NET.Printer;

namespace Kassa.Avalonia.Services;
internal static class EscPosUsbPrinterContainer
{
    private static Printer? _printer;

    public static Printer? Printer
    {
        get => Volatile.Read(ref _printer);
        set => Volatile.Write(ref _printer, value);
    }
}
