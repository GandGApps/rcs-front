using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Interaction logic for ClientView.xaml
/// </summary>
public partial class ClientView : ReactiveUserControl<ClientViewModel>
{

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
               nameof(Command), typeof(ICommand), typeof(ClientView));

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
              nameof(CommandParameter), typeof(object), typeof(ClientView));


    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public ClientView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            Command = ViewModel!.SelectClientCommand!;

            this.OneWayBind(ViewModel, x => x.FullName, x => x.FullName.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Phone, x => x.Phone.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Address, x => x.Address.Text)
                .DisposeWith(disposables);

            ViewModel.WhenAnyValue(x => x.IsDetailsVisible)
                .Subscribe(isVisible =>
                {
                    FullName.SetValue(Grid.RowSpanProperty, isVisible ? 1 : 2);
                    Details.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                })
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsSelected, x => x.IsSelectedIcon.Visibility,
                isSelected => isSelected ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsSelected, x => x.Background, isSelected =>
                {
                    if (isSelected)
                    {
                        var brush = App.GetThemeResource("ClientViewBackgroundSelected");

                        return (Brush)brush;
                    }
                    else
                    {
                        var brush = App.GetThemeResource("ClientViewBackground");

                        return (Brush)brush;
                    }
                })
                .DisposeWith(disposables);
        });
    }
}