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
/// Interaction logic for CommentDialog.xaml
/// </summary>
public partial class CommentDialog : ClosableDialog<CommentDialogViewModel>
{
    public CommentDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, x => x.Comment, x => x.CommentTextBox.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.PublishCommentCommand, x => x.PublishCommentButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.Keyboard.Visibility,
                visibility => visibility ? Visibility.Visible : Visibility.Collapsed
            ).DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.KeyboardVisibilityText.Text,
                x => x ? "Вкл" : "Выкл"
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.IsKeyboardEnabled.IsChecked
            ).DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Comment, x => x.Keyboard.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.CloseCommand, x => x.BackButton)
                .DisposeWith(disposables);

            Keyboard.TextBox = CommentTextBox;
        });
    }
}
