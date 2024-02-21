using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class CashierPaymentPageVm : PageViewModel
{

    public CashierPaymentPageVm(MainViewModel mainViewModel) : base(mainViewModel)
    {
        SendReceiptCommand = ReactiveCommand.Create(() => { });
        EnableWithCheckboxCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!WithReceipt)
            {
                await mainViewModel.DialogOpenCommand
                    .Execute(new SendReceiptDialogViewModel(mainViewModel, this))
                    .FirstAsync();
            }
            else
            {
                IsEmail = false;
                IsPrinter = false;
            }
        });
    }

    [Reactive]
    public ICashierPaymentService CashierPaymentService
    {
        get; private set;
    } = null!;

    [Reactive]
    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get; set;
    }

    [Reactive]
    public double Subtotal
    {
        get; set;
    }

    [Reactive]
    public double Total
    {
        get; set;
    }

    [Reactive]
    public string? Email
    {
        get; set;
    } = string.Empty;

    public extern bool WithReceipt
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public bool IsEmail
    {
        get; set;
    }

    [Reactive]
    public bool IsPrinter
    {
        get; set;
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = "₽";


    public extern string ReceiptActionText
    {
        [ObservableAsProperty]
        get;
    }

    public extern string ReceiptActionIcon
    {
        [ObservableAsProperty]
        get;
    }

    public extern double ToEnter
    {
        [ObservableAsProperty]
        get;
    }


    public extern double ToEntered
    {
        [ObservableAsProperty]
        get;
    }

    public extern double Change
    {
        [ObservableAsProperty]
        get;
    }

    public ReactiveCommand<Unit, Unit> SendReceiptCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> EnableWithCheckboxCommand
    {
        get;
    }

    public ObservableCollection<CashierPaymentItemVm> CashierPaymentItemVms
    {
        get;
    } = [new() { Cost = 123, Name = "DS", CurrencySymbol = "₽" }];


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        CashierPaymentService = await GetInitializedService<ICashierPaymentService>();

        CashierPaymentService.CashierService
            .BindShoppingListItems(x => new ProductShoppingListItemViewModel(x), out var shoppingListItems)
            .DisposeWith(disposables);

        ShoppingListItems = shoppingListItems;

        ShoppingListItems.ToObservableChangeSet()
                         .AutoRefresh(x => x.SubtotalSum)
                         .ToCollection()
                         .Select(x => x.Sum(x => x.SubtotalSum))
                         .Subscribe(x => Subtotal = x)
                         .DisposeWith(disposables);

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, (email, printer) => email || printer)
            .ToPropertyEx(this, x => x.WithReceipt)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.Subtotal)
            .Subscribe(x => Total = x)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, (email, printer) =>
            {
                if (email)
                {
                    return "Отправить на почту";
                }
                else if (printer)
                {

                    return "Переслать чек";
                }
                else
                {
                    return "Распечатать, переслать чек";
                }
            })
            .ToPropertyEx(this, x => x.ReceiptActionText)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, (email, printer) =>
            {
                if (email)
                {
                    return "EmailIcon";
                }
                else if (printer)
                {
                    return "PrinterIcon";
                }
                else
                {
                    return "PrinterIcon";
                }
            })
            .ToPropertyEx(this, x => x.ReceiptActionIcon)
            .DisposeWith(disposables);


        this.WhenAnyValue(x => x.CashierPaymentItemVms, (paymentItems) => paymentItems.Sum(x => x.Cost))
            .ToPropertyEx(this, x => x.ToEntered)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.ToEntered, x => x.Total, (entered, total) => Math.Max(0, total - entered))
            .ToPropertyEx(this, x => x.ToEnter)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.ToEnter, x => x.ToEntered, (toEnter, entered) => Math.Max(0, entered - toEnter))
            .ToPropertyEx(this, x => x.Change)
            .DisposeWith(disposables);

    }
}
