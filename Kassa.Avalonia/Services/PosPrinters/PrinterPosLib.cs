using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;

namespace Kassa.Avalonia.Services.PosPrinters;
public enum PrinterPosLib
{
    None,
    /// <summary>
    /// <see cref="IPrinter"/> implementation is <see cref="EscPosPrinter"/>
    /// </summary>
    Escpos,
    /// <summary>
    /// <see cref="IPrinter"/> implementation is <see cref="EscPosUsbPrinter"/>
    /// </summary>
    EscposUsb,
}
