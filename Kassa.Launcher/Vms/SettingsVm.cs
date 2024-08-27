using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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
        EscPosUsbCashDrawer = CashDrawerVm.EscPosUsbCashDrawer;
        OposCashDrawer = CashDrawerVm.OposCashDrawer;

        Printers = new ReadOnlyObservableCollection<PrinterVm>(_printers);

        foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
        {
            _printers.Add(new(printer));
        }

        SelectedPrinter = _printers[0];

        this.WhenAnyValue(x => x.EscPosUsbCashDrawer.IsSelected, x => x.OposCashDrawer.IsSelected)
            .Select<(bool IsEscPosUsbCashDrawer, bool IsOposCashDrawer), string> (selectedCashDrawer => selectedCashDrawer.IsEscPosUsbCashDrawer 
            ? "Через принтер" 
            : selectedCashDrawer.IsOposCashDrawer 
                ? "Через Opos"
                : "Без денежного ящика")
            .ToPropertyEx(this, x => x.SelectedCashDrawer);

        SelectPrinterCommand = ReactiveCommand.Create<PrinterVm>(printer =>
        {
            foreach (var p in _printers)
            {
                p.IsSelected = false;
            }

            printer.IsSelected = true;

            SelectedPrinter = printer;
        });

        SelectCashDrawerCommand = ReactiveCommand.Create<CashDrawerVm>(cashDrawer =>
        {
            var isSelected = cashDrawer.IsSelected;

            // unselect all
            OposCashDrawer.IsSelected = false;
            EscPosUsbCashDrawer.IsSelected = false;

            if (!isSelected)
            {
                cashDrawer.IsSelected = true;
            }
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
            else
            {
                optionManager.SaveOption("PrinterPosLib", "None");
            }

            if (EscPosUsbCashDrawer.IsSelected)
            {
                optionManager.SaveOption("CashDrawerPosLib", "WndPosLib");
            }
            else if (OposCashDrawer.IsSelected)
            {
                optionManager.SaveOption("CashDrawerPosLib", "EscposUsb");
            }
            else
            {
                optionManager.SaveOption("CashDrawerPosLib", "None");
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

    public ReactiveCommand<bool, Unit> SelectCardReaderMethodCommand
    {
        get;
    }

    public ReactiveCommand<CashDrawerVm, Unit> SelectCashDrawerCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SaveSettingsCommand
    {
        get;
    }

    public CashDrawerVm EscPosUsbCashDrawer
    {
        get;
    }

    public CashDrawerVm OposCashDrawer
    {
        get;
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

    public extern string SelectedCashDrawer
    {
        [ObservableAsProperty]
        get;
    }
}
