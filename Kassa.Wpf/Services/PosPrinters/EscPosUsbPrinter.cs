﻿using System;
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
using Kassa.Shared.ServiceLocator;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Drawing;
using System.IO;
using System.Windows.Media.Media3D;

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
            return new Printer(printerName, "utf-8");
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
        var productService = RcsLocator.Scoped.GetRequiredService<IProductService>();
        var additiveService = RcsLocator.Scoped.GetRequiredService<IAdditiveService>();

        var stackPanel = new StackPanel()
        {
            Width = 48 * 3,
        };

        var textBlock = new TextBlock() { FontFamily = App.LucidaConsoleFont, FontSize = 12, Text = "Кто проитает тот л" };

        stackPanel.Children.Add(textBlock);

        foreach (var orderedProduct in order.Products)
        {
            productIndex++;
            var product = productService.RuntimeProducts[orderedProduct.ProductId];
            stackPanel.Children.Add(new TextBlock()
            {
                FontFamily = App.LucidaConsoleFont,
                FontSize = 12,
                Text = $"{productIndex}){product.Name} {orderedProduct.Count}{product.Measure} {orderedProduct.TotalPrice}{product.CurrencySymbol}"
            });

            foreach (var orderedAdditive in orderedProduct.Additives)
            {
                var additive = additiveService.RuntimeAdditives[orderedAdditive.AdditiveId];
                stackPanel.Children.Add(new TextBlock()
                {
                    FontFamily = App.LucidaConsoleFont,
                    FontSize = 12,
                    Text = $"    {additive.Name} {orderedAdditive.Count}{orderedAdditive.Measure} {orderedAdditive.TotalPrice}{additive.CurrencySymbol}"
                });
            }
        }

        /*printer.Append("Кто прочитает тот л");


        foreach (var orderedProduct in order.Products)
        {
            productIndex++;
            var product = productService.RuntimeProducts[orderedProduct.ProductId];
            var utf8 = Encoding.UTF8.GetBytes($"{productIndex}){product.Name} {orderedProduct.Count}{product.Measure} {orderedProduct.TotalPrice}{product.CurrencySymbol}");
            printer.Append(utf8);

            foreach (var orderedAdditive in orderedProduct.Additives)
            {
                var additive = additiveService.RuntimeAdditives[orderedAdditive.AdditiveId];
                utf8 = Encoding.UTF8.GetBytes($"    {additive.Name} {orderedAdditive.Count}{orderedAdditive.Measure} {orderedAdditive.TotalPrice}{additive.CurrencySymbol}");
                printer.Append(utf8);
            }
        }

        // Добавляем бумагу для отрыва */

        var renderTargetBitmap = new RenderTargetBitmap(48*3, (int)stackPanel.ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
        renderTargetBitmap.Render(stackPanel);

        Bitmap bitmap;

        using (var stream = new MemoryStream())
        {
            // Сохраните изображение в формате PNG
            var pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            pngBitmapEncoder.Save(stream);

            // Преобразуйте поток в Bitmap
            stream.Seek(0, SeekOrigin.Begin);
            bitmap = new Bitmap(stream);
        }

        printer.Image(bitmap);

        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");


        /*// ESC i C 2 - отрыв бумаги
        printer.Append(new byte[] { 0x1B, 0x69, 0x43, 0x02 });

        // Отключая звук ESC B NUL
        printer.Append(new byte[] { 0x1B, 0x42, 0x00 });

        // Отключаем звук ESC BEL NUL
        printer.Append(new byte[] { 0x1B, 0x07, 0x00 });*/

        printer.PrintDocument();
    }
}
