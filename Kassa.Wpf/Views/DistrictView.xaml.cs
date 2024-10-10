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
using Kassa.RxUI;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Interaction logic for DistrictView.xaml
/// </summary>
public partial class DistrictView : ReactiveUserControl<DistrictViewModel>
{
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
                      nameof(Command), typeof(ICommand), typeof(DistrictView));

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
                  nameof(CommandParameter), typeof(object), typeof(DistrictView));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public DistrictView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            this.OneWayBind(ViewModel, x => x.SelectCommand, x => x.Command)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Name, x => x.ModelName.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsSelected, x => x.IsSelectedIcon.Visibility)
                .DisposeWith(disposables);
        });
    }
}
