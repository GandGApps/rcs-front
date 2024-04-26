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
/// Логика взаимодействия для SearchProductDialog.xaml
/// </summary>
public partial class SearchProductDialog : ClosableDialog<SearchProductDialogViewModel>
{
    public SearchProductDialog()
    {
        InitializeComponent();

        var loaded = false;

        this.WhenActivated(disposables =>
        {
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
                vm => vm.SearchText,
                v => v.SearchTextBox.Text
            ).DisposeWith(disposables);

            this.Bind(ViewModel,
                vm => vm.SearchText,
                v => v.Keyboard.Text
            ).DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.KeyboardVisibilityText.Text,
                x => x ? "Вкл" : "Выкл"
            ).DisposeWith(disposables);

            async void action(object? sender, EventArgs args)
            {
                if (!loaded && (ActualHeight > 0 || ActualWidth > 0))
                {

                    await Task.Delay(300);

                    this.OneWayBind(ViewModel, x => x.FilteredItems, x => x.Products.ItemsSource)
                        .DisposeWith(disposables);

                    loaded = true;
                }
            }

            LayoutUpdated += action;

            Disposable.Create(() => LayoutUpdated -= action).DisposeWith(disposables);

        });
    }

    private void ClearSearchText(object sender, MouseButtonEventArgs e)
    {
        ViewModel!.SearchText = string.Empty;
    }

    private void ScrollViewerGotFocus(object sender, RoutedEventArgs e)
    {
        ViewModel!.IsKeyboardVisible = false;
    }
}
