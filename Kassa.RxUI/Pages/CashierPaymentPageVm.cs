using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class CashierPaymentPageVm : PageViewModel
{

    public CashierPaymentPageVm(MainViewModel mainViewModel) : base(mainViewModel)
    {
    }

    [Reactive]
    public ICashierPaymentService CashierPaymentService
    {
        get; private set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel> ShoppingListItems
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

        this.WhenAnyValue(x => x.Subtotal)
            .Subscribe(x => Total = x)
            .DisposeWith(disposables);
    }
}
