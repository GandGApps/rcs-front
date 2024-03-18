using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Kassa.RxUI.Dialogs;
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Interaction logic for InputNumberDialog.xaml
/// </summary>
public partial class InputNumberDialog : ClosableDialog<InputNumberDialogViewModel>
{

    public InputNumberDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {

            this.Bind(ViewModel, x => x.Input, x => x.Input.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OkCommand, x => x.OkButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Input, x => x.Numpad.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.CancelCommand, x => x.CancelButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.FieldName, x => x.FieldName.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Placeholder, x => x.Placeholder.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Input, x => x.Placeholder.Visibility, x => string.IsNullOrEmpty(x) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed)
                .DisposeWith(disposables);

            var clearKey = new KeyInfo { IsClear = true };
            var backspace = new KeyInfo { IsBackspace = true };

            clearKey.Command = ViewModel.ClearCommand;
            backspace.Command = ViewModel.BackspaceCommand;

            var keyboard = KeyboardInfo.Numpad(backspace, clearKey);
            Numpad.KeyboardInfo = keyboard;
        });
    }
}
