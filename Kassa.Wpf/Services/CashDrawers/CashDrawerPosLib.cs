using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Wpf.Services.PosPrinters;

namespace Kassa.Wpf.Services.CashDrawers;
public enum CashDrawerPosLib
{
    /// <summary>
    /// <see cref="ICashDrawer"/> implementation is <see cref="WndPosCashDrawer"/>
    /// </summary>
    WndPosLib,
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
