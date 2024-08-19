using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.Launcher.Services;
using Octokit;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using SukiUI.Controls;

namespace Kassa.Launcher.Vms;
public sealed class SettingsVm: ReactiveObject
{
    private readonly ObservableCollection<PrinterVm> _printers = [PrinterVm.Empty];

    public ReadOnlyObservableCollection<PrinterVm> Printers
    {
        get;
    }

    [Reactive]
    public PrinterVm SelectedPrinter
    {
        get; set;
    }

    public SettingsVm()
    {
        Printers = new ReadOnlyObservableCollection<PrinterVm>(_printers);

        foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
        {
            _printers.Add(new(printer));
        }

        SelectedPrinter = _printers[0];

        SelectPrinterCommand = ReactiveCommand.Create<PrinterVm>(printer =>
        {
            foreach (var p in _printers)
            {
                p.IsSelected = false;
            }

            printer.IsSelected = true;

            SelectedPrinter = printer;
        });

        SelectCashDrawerMethodCommand = ReactiveCommand.Create<bool>(isOposOrEscPosUsb =>
        {
            IsOposOrEscPosUsbCashDrawer = isOposOrEscPosUsb;
        });

        SelectCardReaderMethodCommand = ReactiveCommand.Create<bool>(isOposOrKeyboard =>
        {
            IsOposOrKeyboardCardReader = isOposOrKeyboard;
        });

        SaveSettingsCommand = ReactiveCommand.Create(() =>
        {
            var optionManager = Locator.Current.GetService<IOptionManager>()!;

            if (SelectedPrinter != PrinterVm.Empty)
            {
                optionManager.SaveOption("PrinterPosLib", "EscposUsb");
                optionManager.SaveOption("EscposUsbPrinterName", SelectedPrinter.Name);
            }

            if (IsOposOrEscPosUsbCashDrawer)
            {
                optionManager.SaveOption("CashDrawerPosLib", "WndPosLib");
            }
            else
            {
                optionManager.SaveOption("CashDrawerPosLib", "EscposUsb");
            }

            if (IsOposOrKeyboardCardReader)
            {
                optionManager.SaveOption("MsrReaderLib", "MsrPos");
            }
            else
            {
                optionManager.SaveOption("MsrReaderLib", "MsrKeyboard");
                optionManager.SaveOption("MsrReaderLibKeyboardPrefix", PrefixCardReaderKeyboard);
                optionManager.SaveOption("MsrReaderLibKeyboardSuffix", SuffixCardReaderKeyboard);
            }
        });
    }

    public ReactiveCommand<PrinterVm, Unit> SelectPrinterCommand
    {
        get;
    }

    public ReactiveCommand<bool, Unit> SelectCashDrawerMethodCommand
    {
        get;
    }

    public ReactiveCommand<bool, Unit> SelectCardReaderMethodCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SaveSettingsCommand
    {
        get;
    }

    [Reactive]
    public bool IsOposOrEscPosUsbCashDrawer
    {
        get; set;
    }

    [Reactive]
    public bool IsOposOrKeyboardCardReader
    {
        get; set;
    }

    [Reactive]
    public string SuffixCardReaderKeyboard
    {
        get; set;
    } = @"\n";

    [Reactive]
    public string PrefixCardReaderKeyboard
    {
        get; set;
    }

}
