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
/// Interaction logic for AllClientsDialog.xaml
/// </summary>
public partial class AllClientsDialog : ClosableDialog<AllClientsDialogViewModel>
{
    public AllClientsDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, x => x.FilteredClients, x => x.ClientsList.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.SearchedText, x => x.SearchTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.Keyboard.Visibility,
                visibility => visibility ? Visibility.Visible : Visibility.Collapsed
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.IsKeyboardEnabled.IsChecked
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.SearchedText,
                v => v.SearchTextBox.Text
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.SearchedText,
                v => v.Keyboard.Text
            ).DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.KeyboardVisibilityText.Text,
                x => x ? "Вкл" : "Выкл"
            ).DisposeWith(disposables);
        });
    }

    private void ClearSearchText(object sender, MouseButtonEventArgs e)
    {
        ViewModel!.SearchedText = string.Empty;
    }

    private void ScrollViewerGotFocus(object sender, RoutedEventArgs e)
    {
        ViewModel!.IsKeyboardVisible = false;
    }
}
