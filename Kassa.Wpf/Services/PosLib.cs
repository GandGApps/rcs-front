using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Wpf.Services;
public enum PosLib
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

    [Obsolete("This implementation don't work")]
    /// <summary>
    /// IPrinter implementation is <see cref="EscPosUsbPrinter"/>
    /// </summary>
    EscposUsb,
}
