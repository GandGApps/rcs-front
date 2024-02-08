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
/// Interaction logic for AreYouSureToTurnOffDialog.xaml
/// </summary>
public partial class AreYouSureToTurnOffDialog : ClosableDialog<AreYouSureToTurnOffDialogViewModel>
{
    public AreYouSureToTurnOffDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, vm => vm.TurnOffCommand, v => v.YesButton)
                           .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.NoButton)
                .DisposeWith(disposables);
        });
    }
}
