using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace Kassa.Wpf.Services.PosPrinters;
internal static class PrinterPosLibServices
{
    
    public static void AddPrinterPosLib(this IServiceCollection services, IConfiguration config)
    {
        var printerPosLibString = config.GetValue<string>(nameof(PrinterPosLib));

        var printerPosLib = Enum.TryParse<PrinterPosLib>(printerPosLibString, true, out var pos) ? pos : PrinterPosLib.Wndpos;

        //Improve this by using injection
        switch (printerPosLib)
        {
            case PrinterPosLib.Wndpos:
                services.AddSingleton<IPrinter>(new WndPosPrinter());
                break;
            case PrinterPosLib.Escpos:

                var port = config.GetValue<string>("EscposPrinterPort");
                if (string.IsNullOrWhiteSpace(port))
                {
                    LogHost.Default.Error("Port for Escpos printer is not set");
                    break;
                }
                services.AddSingleton<IPrinter>(new EscPosPrinter(port));
                break;
            case PrinterPosLib.Wnd:
                var useDefaultPrinter = config.GetValue<bool>("UseDefaultPrinter");
                services.AddSingleton<IPrinter>(new WndPrinter(useDefaultPrinter));
                break;
            case PrinterPosLib.EscposUsb:
                var printerName = config.GetValue<string>("EscposUsbPrinterName");
                var ppp = CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(ppp);
                services.AddSingleton<IPrinter>(new EscPosUsbPrinter(printerName));
                break;
            default:
                break;
        }

        LogHost.Default.Info($"Using PrinterPosLib: {printerPosLib}");
    }

}
