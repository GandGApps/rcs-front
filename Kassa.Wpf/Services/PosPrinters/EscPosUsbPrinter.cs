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
using Kassa.Shared.ServiceLocator;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Drawing;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Documents;
using System.Reflection.Metadata;
using System.Windows.Media;
using System.Xml.Linq;
using System.Drawing.Imaging;

namespace Kassa.Wpf.Services.PosPrinters;

internal sealed class EscPosUsbPrinter : IPrinter, IEnableLogger
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
        var productService = RcsKassa.GetRequiredService<IProductService>();
        var additiveService = RcsKassa.GetRequiredService<IAdditiveService>();

        var border = new Border
        {
            Background = System.Windows.Media.Brushes.White,
            Padding = new(20)
        };

        var document = new StackPanel
        {
            
        };

        border.Child = document;

        var whoReadIs = new TextBlock
        {
            Text = "Внимание! Данный чек лишь демонстрация. Менять верстку чека можно будет чуть позже. А пока дефолтный вариант!",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 20),
            FontFamily = App.LucidaConsoleFont,
            TextWrapping = TextWrapping.Wrap
        };

        document.Children.Add(whoReadIs);

        foreach (var orderedProduct in order.Products)
        {

            var product = await productService.GetProductById(orderedProduct.ProductId);

            if (product is null)
            {
                continue;
            }

            var line = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = GridLength.Auto },
                }
            };


            var productText = new TextBlock
            {
                Text = $"{productIndex + 1}) {product.Name} {orderedProduct.Count}x{product.Price} = {orderedProduct.TotalPrice}",
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 0),
                FontFamily = App.LucidaConsoleFont,
                TextWrapping = TextWrapping.Wrap
            };

            var productCount = new TextBlock
            {
                Text = $" {orderedProduct.Count}x{product.Price} = ",
                FontSize = 12,
                TextAlignment = TextAlignment.Right,
                FontFamily = App.LucidaConsoleFont,
                TextWrapping = TextWrapping.Wrap
            };

            Grid.SetColumn(productCount, 1);

            line.Children.Add(productText);
            //line.Children.Add(productCount);

            /*var totalProductPrice = new TextBlock
            {
                Text = $"{orderedProduct.TotalPrice}",
                FontSize = 12.3,
                Margin = new Thickness(0, 0, 0, 0),
                FontWeight = FontWeights.SemiBold,
                FontFamily = App.LucidaConsoleFont,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Right
            };*/

            document.Children.Add(line);
            //document.Children.Add(totalProductPrice);

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
                    FontFamily = App.LucidaConsoleFont,
                    TextWrapping = TextWrapping.Wrap
                };

                document.Children.Add(additiveText);
            }

            productIndex++;
        }

        var subtotal = new TextBlock
        {
            Text = $"Под итог: {order.SubtotalSum}",
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 20, 0, 0),
            FontFamily = App.LucidaConsoleFont,
            TextWrapping = TextWrapping.Wrap
        };

        var total = new TextBlock
        {
            Text = $"Итого: {order.TotalSum}",
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 5, 0, 0),
            FontFamily = App.LucidaConsoleFont,
            TextWrapping = TextWrapping.Wrap
        };

        document.Children.Add(subtotal);
        document.Children.Add(total);

        /*printer.Append("Кто прочитает тот л");


        foreach (var orderedProduct in order.Products)
        {
            productIndex++;
            var product = productService.RuntimeProducts[orderedProduct.ProductId];
            var utf8 = Encoding.UTF8.GetBytes($"{productIndex}){product.ModelName} {orderedProduct.Count}{product.Measure} {orderedProduct.TotalPrice}{product.CurrencySymbol}");
            printer.Append(utf8);

            foreach (var orderedAdditive in orderedProduct.Additives)
            {
                var additive = additiveService.RuntimeAdditives[orderedAdditive.AdditiveId];
                utf8 = Encoding.UTF8.GetBytes($"    {additive.ModelName} {orderedAdditive.Count}{orderedAdditive.Measure} {orderedAdditive.TotalPrice}{additive.CurrencySymbol}");
                printer.Append(utf8);
            }
        }

        // Добавляем бумагу для отрыва */

        var bitmap = Render(border, 48 * 7);

        printer.Image(bitmap);

        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");
        printer.Append("\n");


        // ESC i C 2 - отрыв бумаги
        printer.Append(new byte[] { 0x1B, 0x69, 0x43, 0x02 });

        /*// Отключая звук ESC B NUL
        printer.Append(new byte[] { 0x1B, 0x42, 0x00 });

        // Отключаем звук ESC BEL NUL
        printer.Append(new byte[] { 0x1B, 0x07, 0x00 });*/

        printer.PrintDocument();
    }

    private static Bitmap Render(FrameworkElement visual, double width)
    {
        visual.Width = width;

        visual.Measure(new System.Windows.Size(width, double.PositiveInfinity));
        visual.Arrange(new Rect(new System.Windows.Size(width, visual.DesiredSize.Height)));

        var rtb = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(visual);

        return ConvertBitmapSourceToBitmap(rtb);
    }

    private static Bitmap Render(FlowDocument flowDocument, double width)
    {
        var documentPaginatorSource = (IDocumentPaginatorSource)flowDocument;

        var viewer = new FlowDocumentPageViewer
        {
            Document = documentPaginatorSource,
            Width = width,  // Устанавливаем ширину, высота будет автоматически подстроена
        };

        // Принудительно измеряем и устраиваем визуализацию контента
        viewer.Measure(new System.Windows.Size(width, double.PositiveInfinity));
        viewer.Arrange(new Rect(new System.Windows.Size(width, viewer.DesiredSize.Height)));

        // Теперь захватываем рендер как изображение
        var rtb = new RenderTargetBitmap((int)viewer.ActualWidth, (int)viewer.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(viewer);

        return ConvertBitmapSourceToBitmap(rtb);
    }

    private static Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapSource)
    {
        var bmp = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        var data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
        bmp.UnlockBits(data);
        return bmp;
    }
}
