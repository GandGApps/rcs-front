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
/// Interaction logic for NewDeliveryPage.xaml
/// </summary>
public partial class NewDeliveryPage : ReactiveUserControl<NewDeliveryPageVm>
{
    public NewDeliveryPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.FullName, v => v.FullName.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Phone, v => v.Phone.Text)
                .DisposeWith(disposables);
        });
    }
}
