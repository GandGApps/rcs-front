using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ESC_POS_USB_NET.Printer;
using Splat;
using Kassa.BuisnessLogic;
using Kassa.Shared;

namespace Kassa.Wpf.Services.PosPrinters;

internal sealed class EscPosUsbPrinter : IPrinter, IEnableLogger, IDevelopmentDiagnostics
{
    internal readonly string _printerName;

    public EscPosUsbPrinter(string printerName)
    {
        _printerName = printerName;

        EscPosUsbPrinterContainer.Printer = GetPrinter(printerName);
    }

    public static Printer? GetPrinter(string printerName)
    {
        try
        {
            return new Printer(printerName);
        }
        catch (Exception ex)
        {
            LogHost.Default.Error(ex, $"Error on creating printer");
            return null;
        }
    }

    public ValueTask<bool> CheckService()
    {
        try
        {
            var printer = new Printer(_printerName);

            return new(true);
        }
        catch (Exception ex)
        {
            this.Log().Error(ex, $"Error on creating printer");
            return new(false);
        }
    }

    public async Task PrintAsync(ReportShiftDto reportShift)
    {

    }

    public async Task PrintAsync(OrderDto order)
    {
        var printer = GetPrinter(_printerName);

        if (printer == null)
        {
            return;
        }

        var productIndex = 0;
        var productService = await Locator.Current.GetInitializedService<IProductService>();
        var additiveService = await Locator.Current.GetInitializedService<IAdditiveService>();

        printer.Append("Кто прочитает тот л");

        foreach (var orderedProduct in order.Products)
        {
            productIndex++;
            var product = productService.RuntimeProducts[orderedProduct.ProductId];

            printer.Append($"{productIndex}){product.Name} {orderedProduct.Count}{product.Measure} {orderedProduct.TotalPrice}{product.CurrencySymbol}");

            foreach (var orderedAdditive in orderedProduct.Additives)
            {
                var additive = additiveService.RuntimeAdditives[orderedAdditive.AdditiveId];

                printer.Append($"    {additive.Name} {orderedAdditive.Count}{orderedAdditive.Measure} {orderedAdditive.TotalPrice}{additive.CurrencySymbol}");
            }
        }

        printer.Append(new byte[] { 0x1B, 0x69, 0x43, 0x02 });

        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");


        printer.PrintDocument();


    }
}
