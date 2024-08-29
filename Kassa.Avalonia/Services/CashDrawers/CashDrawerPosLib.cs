using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Avalonia.Services.PosPrinters;

namespace Kassa.Avalonia.Services.CashDrawers;
public enum CashDrawerPosLib
{
    None,
    /// <summary>
    /// <see cref="ICashDrawer"/> implementation is <see cref="CashDrawers.RawSerialPort"/>
    /// </summary>
    RawSerialPort,
    /// <summary>
    /// <see cref="ICashDrawer"/> implementation is <see cref="CashDrawers.EscposUsb"/>
    /// </summary>
    /// <remarks>
    /// WARNING: This implementation needs to be used with <see cref="PrinterPosLib.EscposUsb"/>
    /// </remarks>
    EscposUsb,
}
