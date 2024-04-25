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
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для PincodePage.xaml
/// </summary>
public partial class PincodePage : ReactiveUserControl<PincodePageVm>
{
    public PincodePage()
    {
        InitializeComponent();

        Keyboard.KeyboardInfo = KeyboardInfo.IntegerNumpadWithDelivery();

        CloseButton.DataContext = KeyInfo.Char(' ');
        TechSupport.DataContext = KeyInfo.Char(' ');

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.Pincode, view => view.Keyboard.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.RestoranName, view => view.RestoranName.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Pincode.Length, v => v.StarsCount.Text, count => Helper.GetStars(count))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.LicenseEndDate, view => view.LicenseEndDate.Text, date => date.ToString("dd.MM.yy"))
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.CloseCommand, view => view.CloseButton)
                .DisposeWith(disposables);
        });
    }

    
}
