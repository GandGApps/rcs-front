using System.Reactive.Disposables;
using System.Windows.Media;
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

            this.OneWayBind(ViewModel, x => x.CashierPaymentItemVms, x => x.CashierPaymentItems.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Subtotal, x => x.SubtotalCost.Text, x => $"{x} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Total, x => x.TotalCost.Text, x => $"{x} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.WithReceipt, x => x.HasReceipt.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.WithReceipt, x => x.ActionWithReceipt.IsHitTestVisible)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ReceiptActionText, x => x.ActionWithReceiptText.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x=> x.ReceiptActionIcon, x => x.ActionWithReceipt.Icon, icon => (Geometry)App.Current.FindResource(icon))
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EnableWithCheckboxCommand, x => x.HasReceipt)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ToEnter, x => x.ToEnter.Text, x => $"{x} {ViewModel!.CurrencySymbol}")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ToEntered, x => x.ToEntered.Text, x => $"{x} {ViewModel!.CurrencySymbol}")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Change, x => x.Change.Text, x => $"{x} {ViewModel!.CurrencySymbol}")
                .DisposeWith(disposables);
        });
    }
}
