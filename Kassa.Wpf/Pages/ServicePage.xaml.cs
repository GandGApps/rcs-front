using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Interaction logic for ServicePageVm.xaml
/// </summary>
public partial class ServicePage : ReactiveUserControl<ServicePageVm>
{
    public ServicePage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            Debug.Assert(ViewModel != null);

            this.BindCommand(ViewModel, vm => vm.GoBackCommand, v => v.BackButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm =>  vm.CashierShiftButtonText, v => v.CloseShiftButtonText.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.DepositMoneyCommand, v => v.DepositMoneyButton.Command)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.ShiftButtonCommand, v => v.CloseShiftButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.WithdrawMoneyCommand, v => v.WithdrawMoneyButton)
                .DisposeWith(disposables);

            DataGridOpenOrders.ItemsSource = ViewModel.OpenOrders;
            DataGridClosedOrders.ItemsSource = ViewModel.ClosedOrders;
            DataGridClosedShiftClosedOrder.ItemsSource = ViewModel.OrdersOfClosedCashShifts;

        });
    }
}
