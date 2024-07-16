using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.Wpf.Services;
internal static class PrinterPosLibSplatExtensions
{

    public static void AddPrinterPosLib(this IMutableDependencyResolver services, IConfiguration config)
    {
        var printerPosLibString = config.GetValue<string>(nameof(PrinterPosLib));

        var printerPosLib = Enum.TryParse<PrinterPosLib>(printerPosLibString, true, out var pos) ? pos : PrinterPosLib.Wndpos;

        switch (printerPosLib)
        {
            case PrinterPosLib.Wndpos:
                services.RegisterConstantAndDiagnose<IPrinter>(new WndPosPrinter());
                break;
            case PrinterPosLib.Escpos:

                var port = config.GetValue<string>("EscposPrinterPort");
                if (string.IsNullOrWhiteSpace(port))
                {
                    LogHost.Default.Error("Port for Escpos printer is not set");
                    break;
                }
                services.RegisterConstantAndDiagnose<IPrinter>(new EscPosPrinter(port));
                break;
            case PrinterPosLib.Wnd:
                var useDefaultPrinter = config.GetValue<bool>("UseDefaultPrinter");
                services.RegisterConstantAndDiagnose<IPrinter>(new WndPrinter(useDefaultPrinter));
                break;
            // TODO: Fix or remove this implementation
            case PrinterPosLib.EscposUsb:
                var printerName = config.GetValue<string>("EscposUsbPrinterName");
                services.RegisterConstantAndDiagnose<IPrinter>(new EscPosUsbPrinter(printerName));
                break;
            default:
                break;
        }

        LogHost.Default.Info($"Using PrinterPosLib: {printerPosLib}");
    }

}
