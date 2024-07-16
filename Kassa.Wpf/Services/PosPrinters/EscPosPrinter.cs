using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Splat;

namespace Kassa.Wpf.Services.PosPrinters;
internal sealed class EscPosPrinter : BuisnessLogic.Services.IPrinter, IEnableLogger
{
    private readonly BasePrinter? _printer;

    public EscPosPrinter(string port)
    {
        if (string.IsNullOrEmpty(port))
        {
            this.Log().Warn("No port specified, need add in configuration");
            return;
        }

        try
        {
            _printer = new SerialPrinter(port, 9600);
        }
        catch (Exception exc)
        {
            this.Log().Error(exc, "Error creating printer");
        }


    }

    public async Task PrintAsync(ReportShiftDto reportShift)
    {
        if (_printer == null)
        {
            this.Log().Warn("No printer found");
            return;
        }

        var e = new EPSON();
        var commands = new List<byte[]>
        {
            e.Initialize(),
            e.CenterAlign(),
            e.PrintLine("-----------------------------------------"),
            e.PrintLine("Торговое"),
            e.PrintLine("ИНН"),
            e.PrintLine("-----------------------------------------"),
            e.PrintLine(string.Empty),
            e.PrintLine("Отчет о смене"),
            e.PrintLine(CenterText("X-отчет", 48)),
            e.PrintLine(LeftAndRightText("Терминал: ", "№9101 (Криспи Гриль Эсамбаева 4 Тестовый)", 48)),
            e.PrintLine(LeftAndRightText("Кассовая смена:", "5", 48)),
            e.PrintLine(LeftAndRightText("Смена открыта:", "15.01.2024 20:34", 48)),
            e.PrintLine(LeftAndRightText("Текущее время:", "02.02.2024 12:42", 48)),
            e.PrintLine(LeftAndRightText("Текущий пользователь:", "Григор", 48)),
            e.PrintLine("Наличные"),
            e.PrintLine(LeftAndRightText("Продажа", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Возврат", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Покупка", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Возвр. покупки", "75,00", 48)),
            e.PrintLine(string.Empty),
            e.PrintLine("Сбербанк"),
            e.PrintLine(LeftAndRightText("Продажа", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Возврат", "75,00", 48)),
            e.PrintLine(string.Empty),
            e.PrintLine("Итого"),
            e.PrintLine(LeftAndRightText("Продажа", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Возврат", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Покупка", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Возвр. покупки", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Подкрепление", "75,00", 48)),
            e.PrintLine(LeftAndRightText("Инкассация", "75,00", 48)),
            e.PrintLine(string.Empty),
            e.PrintLine(CenterText("ВЫПОЛНЕНО ОПЕРАЦИЙ", 48)),
            e.PrintLine(LeftAndRightText("Продажа", "1", 48)),
            e.PrintLine(LeftAndRightText("Возврат", "1", 48)),
            e.PrintLine(LeftAndRightText("Покупка", "1", 48)),
            e.PrintLine(LeftAndRightText("Возвр. покупки", "0", 48)),
            e.PrintLine(LeftAndRightText("Подкрепление", "0", 48)),
            e.PrintLine(LeftAndRightText("Инкассация", "0", 48)),
            e.PrintLine(string.Empty),
            e.PrintLine("Внимание! Приведенные суммы могут отличаться от сумм фискального регистратора!"),
            e.FullCut()
        };

        _printer.Write(commands.ToArray());
    }

    public async Task PrintAsync(OrderDto order)
    {
        if (_printer == null)
        {
            this.Log().Warn("No printer found");
            return;
        }

        var e = new EPSON();
        var commands = new List<byte[]>
        {
            e.Initialize(),
            e.CenterAlign()
        };

        var productIndex = 0;
        var productService = await Locator.Current.GetInitializedService<IProductService>();
        var additiveService = await Locator.Current.GetInitializedService<IAdditiveService>();

        foreach (var orderedProduct in order.Products)
        {
            productIndex++;
            var product = productService.RuntimeProducts[orderedProduct.ProductId];

            commands.Add(e.PrintLine($"{productIndex}) {product.Name} {orderedProduct.Count}{product.Measure} {orderedProduct.TotalPrice}{product.CurrencySymbol}"));

            foreach (var orderedAdditive in orderedProduct.Additives)
            {
                var additive = additiveService.RuntimeAdditives[orderedAdditive.AdditiveId];
                commands.Add(e.PrintLine($"    {additive.Name} {orderedAdditive.Count}{orderedAdditive.Measure} {orderedAdditive.TotalPrice}{additive.CurrencySymbol}"));
            }
        }

        commands.Add(e.FullCut());
        _printer.Write(commands.ToArray());
    }

    private static string CenterText(string text, int maxChars)
    {
        var padSize = (maxChars - text.Length) / 2;
        return new string(' ', padSize) + text;
    }

    private static string LeftAndRightText(string left, string right, int maxChars)
    {
        var totalSize = left.Length + right.Length;
        var spaces = maxChars - totalSize;
        return $"{left}{new string(' ', spaces)}{right}";
    }
}
