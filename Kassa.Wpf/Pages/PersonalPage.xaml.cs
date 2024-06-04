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
/// Interaction logic for PersonalPage.xaml
/// </summary>
public partial class PersonalPage : ReactiveUserControl<PersonalPageVm>
{
    public PersonalPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, vm => vm.TakeBreakCommand, v => v.TakeBreakButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ShiftButtonText, v => v.ShiftButtonText.Text)
                .DisposeWith(disposables);

            /*this.OneWayBind(ViewModel, vm => vm.ManagerName, v => v.ManagerName.Text)
                .DisposeWith(disposables);
            
            this.OneWayBind(ViewModel, vm => vm.CashierName, v => v.CashierName.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ShiftNumber, v => v.ShiftNumber.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.OpennedShiftDate, v => v.ShiftBegin.Text)
                .DisposeWith(disposables);*/

            this.BindCommand(ViewModel, vm => vm.ShiftCommand, v => v.ShiftButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.SelectedShifts, v => v.Orders.ItemsSource)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.GoBackCommand, v => v.BackButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.IsOpennedShiftsVisible, v => v.OpenShifts.IsChecked)
                .DisposeWith(disposables);

            /*this.OneWayBind(ViewModel, vm => vm.IsShiftStarted, v => v.ShiftState.Text, x => x ? " открыта " : " закрыта ")
                .DisposeWith(disposables);*/

        });
    }
}
