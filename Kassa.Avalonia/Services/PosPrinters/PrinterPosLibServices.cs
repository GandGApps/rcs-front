using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.Avalonia.Services.PosPrinters;
internal static class PrinterPosLibServices
{

    public static void RegisterPrinterPosLib(IConfiguration config)
    {
        var printerPosLibString = config.GetValue<string>(nameof(PrinterPosLib));

        var printerPosLib = Enum.TryParse<PrinterPosLib>(printerPosLibString, true, out var pos) ? pos : PrinterPosLib.None;

        switch (printerPosLib)
        {
            case PrinterPosLib.Escpos:

                var port = config.GetValue<string>("EscposPrinterPort");
                if (string.IsNullOrWhiteSpace(port))
                {
                    LogHost.Default.Error("Port for Escpos printer is not set");
                    break;
                }
                RcsLocatorBuilder.AddSingleton<IPrinter>(new EscPosPrinter(port));
                break;
            case PrinterPosLib.EscposUsb:
                var printerName = config.GetValue<string>("EscposUsbPrinterName")!;
                var ppp = CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(ppp);
                RcsLocatorBuilder.AddSingleton<IPrinter>(new EscPosUsbPrinter(printerName));
                break;
            default:
                break;
        }

        LogHost.Default.Info($"Using PrinterPosLib: {printerPosLib}");
    }

}
