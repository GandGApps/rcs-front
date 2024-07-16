using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Wpf.Services;
public enum PrinterPosLib
{
    /// <summary>
    /// IPrinter implementation is <see cref="WndPrinter"/>
    /// </summary>
    Wnd, 

    /// <summary>
    /// IPrinter implementation is <see cref="WndPosPrinter"/>
    /// </summary>
    Wndpos,

    /// <summary>
    /// IPrinter implementation is <see cref="EscPosPrinter"/>
    /// </summary>
    Escpos,
    /// <summary>
    /// IPrinter implementation is <see cref="EscPosUsbPrinter"/>
    /// </summary>
    EscposUsb,
}
