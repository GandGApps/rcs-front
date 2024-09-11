using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kassa.Avalonia.Dialogs;
using Kassa.RxUI.Dialogs;

namespace Kassa.Avalonia;

public sealed partial class InputDialog : ClosableDialog<InputDialogViewModel>
{
    public InputDialog()
    {
        InitializeComponent();
    }
}