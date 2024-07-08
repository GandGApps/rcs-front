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

namespace Kassa.Wpf.Services;


[Obsolete("This implementation don't work")]
/// <summary>
/// Don't work, need to be fixed or 
/// removed from the project
/// </summary>
internal sealed class EscPosUsbPrinter : IPrinter, IEnableLogger
{
    private readonly string _printerName;

    public EscPosUsbPrinter(string printerName)
    {
        _printerName = printerName;
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

        printer.PrintDocument();


    }
}
