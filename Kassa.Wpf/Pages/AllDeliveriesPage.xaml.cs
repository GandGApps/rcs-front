using System;
using System.Collections.Generic;
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
/// Interaction logic for AllDeliveriesPage.xaml
/// </summary>
public partial class AllDeliveriesPage : ReactiveUserControl<AllDeliveriesPageVm>
{
    public AllDeliveriesPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, x => x.Orders, x => x.Orders.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Date, v => v.CurrentDate.DateTime)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.InUncomfiredOrders, v => v.UnconfirmedCount.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.InCookingOrders, v => v.InCookingCount.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.ReadyOrders, v => v.ReadyCount.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.CanceledOrders, v => v.CancelCount.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.OnTheWayOrders, v => v.OnTheWayCount.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.CompletedOrders, v => v.CompleatedCount.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.NewOrders, v => v.NewCount.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.IsDelivery, v => v.IsDelivery.IsChecked)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.IsPickup, v => v.IsPickUp.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.Keyboard.Visibility,
                visibility => visibility ? Visibility.Visible : Visibility.Collapsed
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.IsKeyboardEnabled.IsChecked
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.SearchedText,
                v => v.SearchTextBox.Text
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.SearchedText,
                v => v.Keyboard.Text
            ).DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.KeyboardVisibilityText.Text,
                x => x ? "Вкл" : "Выкл"
            ).DisposeWith(disposables);
        });
    }

    private void ClearSearchText(object sender, MouseButtonEventArgs e)
    {
        ViewModel!.SearchedText = string.Empty;
    }

    private void ScrollViewerGotFocus(object sender, RoutedEventArgs e)
    {
        ViewModel!.IsKeyboardVisible = false;
    }
}
