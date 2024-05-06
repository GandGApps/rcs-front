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

            this.BindCommand(ViewModel, vm => vm.EndShiftCommand, v => v.CloseShiftButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.SelectedShifts, v => v.Orders.ItemsSource)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.GoBackCommand, v => v.BackButton)
                .DisposeWith(disposables);
        });
    }
}
