using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;

namespace Kassa.Wpf.Services.PosPrinters;
public enum PrinterPosLib
{
    /// <summary>
    /// <see cref="IPrinter"/> implementation is <see cref="WndPrinter"/>
    /// </summary>
    Wnd,

    /// <summary>
    /// <see cref="IPrinter"/> implementation is <see cref="WndPosPrinter"/>
    /// </summary>
    Wndpos,

    /// <summary>
    /// <see cref="IPrinter"/> implementation is <see cref="EscPosPrinter"/>
    /// </summary>
    Escpos,
    /// <summary>
    /// <see cref="IPrinter"/> implementation is <see cref="EscPosUsbPrinter"/>
    /// </summary>
    EscposUsb,
}
