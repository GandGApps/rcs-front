using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using System.Drawing.Printing;
using Kassa.BuisnessLogic.Dto;
using System.Drawing;
using System.Windows.Controls;
using System.Printing;
using System.Windows.Documents;
using Splat;
using Kassa.BuisnessLogic;
using System.Windows;

namespace Kassa.Wpf.Services;

/// <summary>
/// Implementation of <see cref="IPrinter"/> for Wnd printer.
/// This implementation use <see cref="PrintDialog"/> for printing.
/// </summary>
internal sealed class WndPrinter(bool useDefaultPrinter) : IPrinter, IEnableLogger
{
    public Task PrintAsync(ReportShiftDto reportShift)
    {
        return Task.CompletedTask;
    }

    public async Task PrintAsync(OrderDto order)
    {
        var printDialog = new PrintDialog();

        if (useDefaultPrinter)
        {
            var printer = LocalPrintServer.GetDefaultPrintQueue();

            this.Log().Info($"Printer found: {printer.Name}");

            printDialog.PrintQueue = printer;
        }

        var document = new FlowDocument();

        var productIndex = 0;
        var productService = await Locator.Current.GetInitializedService<IProductService>();
        var additiveService = await Locator.Current.GetInitializedService<IAdditiveService>();

        foreach (var orderedProduct in order.Products)
        {

            var product = await productService.GetProductById(orderedProduct.ProductId);

            if (product is null)
            {
                continue;
            }

            var productText = new TextBlock
            {
                Text = $"{product.Name} {orderedProduct.Count}x{product.Price}",
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 0),
                FontFamily = App.LucidaConsoleFont
            };

            document.Blocks.Add(new BlockUIContainer(productText));

            foreach (var orderedAdditive in orderedProduct.Additives)
            {

                var additive = await additiveService.GetAdditiveById(orderedAdditive.AdditiveId);

                if (additive is null)
                {
                    continue;
                }

                var additiveText = new TextBlock
                {

                    Text = $"  {additive.Name} {orderedAdditive.Count}x{additive.Price}",
                    FontSize = 10,
                    Margin = new Thickness(0, 0, 0, 0),
                    FontFamily = App.LucidaConsoleFont
                };

                document.Blocks.Add(new BlockUIContainer(additiveText));
            }

            productIndex++;
        }

        var documentPaginatorSource = (IDocumentPaginatorSource)document;

        this.Log().Info("Printing document");

        printDialog.PrintDocument(documentPaginatorSource.DocumentPaginator, "Order");

    }
}
