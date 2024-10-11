using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared.ServiceLocator;
using Kassa.Wpf.Services.PosPrinters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace Kassa.Wpf.Services.CashDrawers;
internal static class CashDrawerPosLibServices
{

    public static void AddCashDrawerPosLib(this IServiceCollection services, IConfiguration config)
    {

        var cashDrawerPosLibString = config.GetValue<string>(nameof(CashDrawerPosLib));

        var cashDrawerPosLib = Enum.TryParse<CashDrawerPosLib>(cashDrawerPosLibString, true, out var pos) ? pos : CashDrawerPosLib.WndPosLib;

        switch (cashDrawerPosLib)
        {

            case CashDrawerPosLib.WndPosLib:
                RcsLocatorBuilder.AddSingleton<ICashDrawer>(new WndPosCashDrawer());
                break;
            case CashDrawerPosLib.RawSerialPort:
                var rawBytesString = config.GetValue($"{nameof(RawSerialPort)}.RawBytes", "00")!;
                byte[] rawBytes;
                try
                {
                    rawBytes = Convert.FromHexString(rawBytesString);
                }
                catch (Exception exc)
                {
                    LogHost.Default.Error(exc, $"Error parsing {nameof(RawSerialPort)}.RawBytes");
                    return;
                }

                var port = config.GetValue<string>($"{nameof(RawSerialPort)}.Port")!;

                if (!SerialPort.GetPortNames().Any(x => x == port))
                {
                    LogHost.Default.Error($"Port {port} not found");
                    return;
                }
                RcsLocatorBuilder.AddSingleton<ICashDrawer>(new RawSerialPort(rawBytes, port));

                break;
            case CashDrawerPosLib.EscposUsb:

                var printerImplementation = config.GetValue<string>(nameof(PrinterPosLib));

                if (config.GetValue<string>(nameof(PrinterPosLib)) != nameof(PrinterPosLib.EscposUsb))
                {
                    LogHost.Default.Error($"PrinterPosLib should be {nameof(PrinterPosLib.EscposUsb)}");
                    return;
                }

                RcsLocatorBuilder.AddSingleton<ICashDrawer>(new EscposUsb());
                break;
            default:
                break;
        }

        LogHost.Default.Info($"Using CashDrawerPosLib: {cashDrawerPosLib}");
    }

}
