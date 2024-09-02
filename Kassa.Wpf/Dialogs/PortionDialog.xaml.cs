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
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Interaction logic for PortionDialog.xaml
/// </summary>
public sealed partial class PortionDialog : ClosableDialog<PortionDialogVm>
{
    public PortionDialog()
    {
        InitializeComponent();

        Numpad.KeyboardInfo = KeyboardInfo.Numpad();

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.IsIntoSeveralEqualParts, v => v.IsIntoSeveralEqualPartsRadioButton.IsChecked)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.CurrentMethodOfDivision.ApplyCommand, x => x.OkButton)
                .DisposeWith(disposables);

            ViewModel.WhenAnyValue(x => x.IsIntoSeveralEqualParts)
                .Subscribe(x =>
                {

                })
                .DisposeWith(disposables);
        });
    }
}