﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Splat;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;

namespace Kassa.Wpf.Services;
internal sealed class Printer : IPrinter
{

    private static async Task<PosPrinter?> FindFirst()
    {
        var deviceSelector = PosPrinter.GetDeviceSelector();
        var devices = await DeviceInformation.FindAllAsync(deviceSelector);

        if (devices.Count == 0)
        {
            return null;
        }

        var deviceInfo = devices[0];
        var printer = await PosPrinter.FromIdAsync(deviceInfo.Id);

        if (printer == null)
        {
            return null;
        }

        return printer;
    }

    public async Task PrintAsync(ReportShiftDto reportShift)
    {
        using var posPrinter = await FindFirst();

        if (posPrinter == null)
        {
            return;
        }

        using var claimedPosPrinter = await posPrinter.ClaimPrinterAsync();
        var job = claimedPosPrinter.Receipt.CreateJob();

        job.PrintLine("-----------------------------------------");
        job.PrintLine("Торговое");
        job.PrintLine("ИНН");
        job.PrintLine("-----------------------------------------");
        job.PrintLine();

        job.PrintLine("Отчет о смене");

        CenterText(claimedPosPrinter, job, "X-отчет");
        LeftAndRightText(claimedPosPrinter, job, "Терминал: ", "№9101 (Криспи Гриль Эсамбаева 4 Тестовый)");
        LeftAndRightText(claimedPosPrinter, job, "Кассовая смена:", "5");
        LeftAndRightText(claimedPosPrinter, job, "Смена открыта:", "15.01.2024 20:34");
        LeftAndRightText(claimedPosPrinter, job, "Текущее время:", "02.02.2024 12:42");
        LeftAndRightText(claimedPosPrinter, job, "Текущий пользователь:", "Григор");

        job.PrintLine("Наличные");
        LeftAndRightText(claimedPosPrinter, job, "Продажа", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Возврат", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Покупка", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Возвр. покупки", "75,00");
        job.PrintLine();

        job.PrintLine("Сбербанк");
        LeftAndRightText(claimedPosPrinter, job, "Продажа", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Возврат", "75,00");
        job.PrintLine();

        job.PrintLine("Итого");
        LeftAndRightText(claimedPosPrinter, job, "Продажа", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Возврат", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Покупка", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Возвр. покупки", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Подкрепление", "75,00");
        LeftAndRightText(claimedPosPrinter, job, "Инкассация", "75,00");
        job.PrintLine();

        CenterText(claimedPosPrinter, job, "ВЫПОЛНЕНО ОПЕРАЦИЙ");
        LeftAndRightText(claimedPosPrinter, job, "Продажа", "1");
        LeftAndRightText(claimedPosPrinter, job, "Возврат", "1");
        LeftAndRightText(claimedPosPrinter, job, "Покупка", "1");
        LeftAndRightText(claimedPosPrinter, job, "Возвр. покупки", "0");
        LeftAndRightText(claimedPosPrinter, job, "Подкрепление", "0");
        LeftAndRightText(claimedPosPrinter, job, "Инкассация", "0");
        job.PrintLine();

        job.PrintLine("Внимание! Приведенные суммы могут отличаться от сумм фискального регистратора!");

        await job.ExecuteAsync();

    }

    public async Task PrintAsync(OrderDto order)
    {
        using var posPrinter = await FindFirst();

        if (posPrinter == null)
        {
            return;
        }

        using var claimedPosPrinter = await posPrinter.ClaimPrinterAsync();
        var job = claimedPosPrinter.Receipt.CreateJob();

        var productIndex = 0;
        var productService = await Locator.Current.GetInitializedService<IProductService>();
        var additiveService = await Locator.Current.GetInitializedService<IAdditiveService>();

        foreach (var orderedProduct in order.Products)
        {
            productIndex++;
            var product = productService.RuntimeProducts[orderedProduct.ProductId];

            job.PrintLine($"{productIndex}){product.Name} {orderedProduct.Count}{product.Measure} {orderedProduct.TotalPrice}{product.CurrencySymbol}");

            foreach (var orderedAdditive in orderedProduct.Additives)
            {
                var additive = additiveService.RuntimeAdditives[orderedAdditive.AdditiveId];

                job.PrintLine($"    {additive.Name} {orderedAdditive.Count}{orderedAdditive.Measure} {orderedAdditive.TotalPrice}{additive.CurrencySymbol}");
            }
        }

        await job.ExecuteAsync();

    }

    private static string CenterText(string text, int maxChars)
    {
        var padSize = (maxChars - text.Length) / 2;
        return new string(' ', padSize) + text;
    }

    private static void CenterText(ClaimedPosPrinter claimedPosPrinter, ReceiptPrintJob receiptPrintJob, string text)
    {
        var maxChars = claimedPosPrinter.Slip.SidewaysMaxChars;

        var centeredText = CenterText(text, Convert.ToInt32(maxChars));

        receiptPrintJob.PrintLine(centeredText);
    }

    private static void LeftAndRightText(ClaimedPosPrinter claimedPosPrinter, ReceiptPrintJob receiptPrintJob, string left, string right)
    {
        var totalSize = left.Length + right.Length;
        var formatedText = $"{left}{new string(' ',totalSize)}{right}";

        receiptPrintJob.PrintLine(formatedText);
    }
}
