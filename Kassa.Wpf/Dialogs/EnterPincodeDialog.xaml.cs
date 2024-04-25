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
using ReactiveUI;
using System.Windows.Shapes;
using Kassa.RxUI.Dialogs;
using System.Reactive.Disposables;
using Kassa.Wpf.Controls;

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Interaction logic for EnterPincodeDialog.xaml
/// </summary>
public partial class EnterPincodeDialog : ClosableDialog<EnterPincodeDialogViewModel>
{
    public EnterPincodeDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Pincode!.Length, x => x.Input.Text, x => Helper.GetStars(x))
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Pincode, x => x.Numpad.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.CancelCommand, x => x.CancelButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Pincode, x => x.Placeholder.Visibility, x => string.IsNullOrEmpty(x) ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

            var clearKey = new KeyInfo { IsClear = true };
            var backspace = new KeyInfo { IsBackspace = true };

            clearKey.Command = ViewModel!.ClearCommand;
            backspace.Command = ViewModel.BackspaceCommand;

            var keyboard = KeyboardInfo.Numpad(backspace, clearKey);
            Numpad.KeyboardInfo = keyboard;
        });
    }
}
