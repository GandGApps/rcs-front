using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Microsoft.PointOfService;
using Splat;

namespace Kassa.Wpf.Services;
internal sealed class McPrinter : IPrinter,IEnableLogger
{
#if MC_PRINTER
    private readonly PosExplorer explorer;
    private PosPrinter printer;
#endif
    public McPrinter()
    {
#if MC_PRINTET
        explorer = new PosExplorer();
#endif
    }

    private async Task<PosPrinter?> FindFirstAsync()
    {
#if MC_PRINTER
        try
        {
            var deviceInfo = explorer.GetDevice("PosPrinter");
            if (deviceInfo != null)
            {
                printer = (PosPrinter)explorer.CreateInstance(deviceInfo);
                printer.Open();
                printer.Claim(1000);
                printer.DeviceEnabled = true;
                LogHost.Default.Info($"Printer found: {deviceInfo.ServiceObjectName}");
                return printer;
            }
        }
        catch (Exception ex)
        {
            LogHost.Default.Error($"Error finding printer: {ex.Message}");
        }
#endif
        return null;
    }

    public async Task PrintAsync(ReportShiftDto reportShift)
    {
#if MC_PRINTER
        var posPrinter = await FindFirstAsync();

        if (posPrinter == null)
        {
            this.Log().Warn("No printer found");
            return;
        }

        this.Log().Info("Printer found");

        var job = new System.Text.StringBuilder();

        job.AppendLine("-----------------------------------------");
        job.AppendLine("Торговое");
        job.AppendLine("ИНН");
        job.AppendLine("-----------------------------------------");
        job.AppendLine();

        job.AppendLine("Отчет о смене");

        job.AppendLine(CenterText("X-отчет"));
        job.AppendLine(LeftAndRightText("Терминал: ", "№9101 (Криспи Гриль Эсамбаева 4 Тестовый)"));
        job.AppendLine(LeftAndRightText("Кассовая смена:", "5"));
        job.AppendLine(LeftAndRightText("Смена открыта:", "15.01.2024 20:34"));
        job.AppendLine(LeftAndRightText("Текущее время:", "02.02.2024 12:42"));
        job.AppendLine(LeftAndRightText("Текущий пользователь:", "Григор"));

        job.AppendLine("Наличные");
        job.AppendLine(LeftAndRightText("Продажа", "75,00"));
        job.AppendLine(LeftAndRightText("Возврат", "75,00"));
        job.AppendLine(LeftAndRightText("Покупка", "75,00"));
        job.AppendLine(LeftAndRightText("Возвр. покупки", "75,00"));
        job.AppendLine();

        job.AppendLine("Сбербанк");
        job.AppendLine(LeftAndRightText("Продажа", "75,00"));
        job.AppendLine(LeftAndRightText("Возврат", "75,00"));
        job.AppendLine();

        job.AppendLine("Итого");
        job.AppendLine(LeftAndRightText("Продажа", "75,00"));
        job.AppendLine(LeftAndRightText("Возврат", "75,00"));
        job.AppendLine(LeftAndRightText("Покупка", "75,00"));
        job.AppendLine(LeftAndRightText("Возвр. покупки", "75,00"));
        job.AppendLine(LeftAndRightText("Подкрепление", "75,00"));
        job.AppendLine(LeftAndRightText("Инкассация", "75,00"));
        job.AppendLine();

        job.AppendLine(CenterText("ВЫПОЛНЕНО ОПЕРАЦИЙ"));
        job.AppendLine(LeftAndRightText("Продажа", "1"));
        job.AppendLine(LeftAndRightText("Возврат", "1"));
        job.AppendLine(LeftAndRightText("Покупка", "1"));
        job.AppendLine(LeftAndRightText("Возвр. покупки", "0"));
        job.AppendLine(LeftAndRightText("Подкрепление", "0"));
        job.AppendLine(LeftAndRightText("Инкассация", "0"));
        job.AppendLine();

        job.AppendLine("Внимание! Приведенные суммы могут отличаться от сумм фискального регистратора!");

        posPrinter.PrintNormal(PrinterStation.Receipt, job.ToString());
#endif
    }

    public async Task PrintAsync(OrderDto order)
    {
#if MC_PRINTER
        var posPrinter = await FindFirstAsync();

        if (posPrinter == null)
        {
            this.Log().Warn("No printer found");
            return;
        }

        this.Log().Info("Printer found");

        var job = new System.Text.StringBuilder();

        var productIndex = 0;
        var productService = await Locator.Current.GetInitializedService<IProductService>();
        var additiveService = await Locator.Current.GetInitializedService<IAdditiveService>();

        foreach (var orderedProduct in order.Products)
        {
            productIndex++;
            var product = productService.RuntimeProducts[orderedProduct.ProductId];

            job.AppendLine($"{productIndex}) {product.Name} {orderedProduct.Count}{product.Measure} {orderedProduct.TotalPrice}{product.CurrencySymbol}");

            foreach (var orderedAdditive in orderedProduct.Additives)
            {
                var additive = additiveService.RuntimeAdditives[orderedAdditive.AdditiveId];

                job.AppendLine($"    {additive.Name} {orderedAdditive.Count}{orderedAdditive.Measure} {orderedAdditive.TotalPrice}{additive.CurrencySymbol}");
            }
        }

        posPrinter.PrintNormal(PrinterStation.Receipt, job.ToString());
#endif
    }

    private static string CenterText(string text)
    {
        var padSize = (48 - text.Length) / 2; // Предполагается, что ширина принтера 48 символов
        return new string(' ', padSize) + text;
    }

    private static string LeftAndRightText(string left, string right)
    {
        var totalSize = 48; // Предполагается, что ширина принтера 48 символов
        var spaces = totalSize - (left.Length + right.Length);
        return $"{left}{new string(' ', spaces)}{right}";
    }
}
