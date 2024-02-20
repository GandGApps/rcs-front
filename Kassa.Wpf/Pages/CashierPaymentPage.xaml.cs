using System.Reactive.Disposables;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для CashierPaymentPage.xaml
/// </summary>
public partial class CashierPaymentPage : ReactiveUserControl<CashierPaymentPageVm>
{
    public CashierPaymentPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, x => x.ShoppingListItems, x => x.ShoppingListItems.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Subtotal, x => x.SubtotalCost.Text, x => $"{x} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Total, x => x.TotalCost.Text, x => $"{x} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.WithReceipt, x => x.HasReceipt.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.WithReceipt, x => x.ActionWithReceipt.IsEnabled)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EnableWithCheckboxCommand, x => x.HasReceipt)
                .DisposeWith(disposables);
        });
    }
}
