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
    public string Email
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

    public ReactiveCommand<Unit, Unit> SendReceiptCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> EnableWithCheckboxCommand
    {
        get;
    }

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
    }
}
