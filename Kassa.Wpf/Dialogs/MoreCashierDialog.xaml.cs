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
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Логика взаимодействия для MoreCashierDialog.xaml
/// </summary>
public partial class MoreCashierDialog : ClosableDialog<MoreCashierDialogViewModel>
{
    public MoreCashierDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, x => x.AddCommentCommand, x => x.AddCommentToProductButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.StopListCommand, x => x.StopListButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShowPriceCommand, x => x.ShowDishesPriceButton)
                .DisposeWith(disposables);
        });
    }
}
