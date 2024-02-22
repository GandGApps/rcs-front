using System.Reactive.Disposables;
using System.Windows.Media;
using Kassa.RxUI;
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
            DataContext = ViewModel;

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

            this.OneWayBind(ViewModel, x => x.ToEnter, x => x.ToEnter.Text, x => $"{x:N2} {ViewModel!.CurrencySymbol}")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ToEntered, x => x.ToEntered.Text, x => $"{x:N2} {ViewModel!.CurrencySymbol}")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Change, x => x.Change.Text, x => $"{x:N2} {ViewModel!.CurrencySymbol}")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.CurrentPaymentSumText, x => x.CurrentPaymentSum.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.CurrencySymbol, x => x.CurrentPaymentSumCurrency.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.CurrentPaymentType, x => x.PaymentTypeName.Text, (type) =>
                {
                    return type switch 
                    {
                        PaymentType.Cash => "Наличные",
                        PaymentType.BankCard => "Банковская карта",
                        PaymentType.CashlessPayment => "Безналичный расчет",
                        PaymentType.WithoutRevenue => "Без выручки",
                        _ => "Неизвестный тип оплаты"
                    };
                })
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SetPaymentTypeCommand, x => x.Cash)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SetPaymentTypeCommand, x => x.BankCard)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SetPaymentTypeCommand, x => x.CashlessPayment)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SetPaymentTypeCommand, x => x.WithoutRevenue)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ExactAmountCommand, x => x.ExactAmount)
                .DisposeWith(disposables);
        });
    }
}
