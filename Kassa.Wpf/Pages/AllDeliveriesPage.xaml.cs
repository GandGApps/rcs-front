using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
