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
            this.OneWayBind(ViewModel, x => x.FullName, x => x.FullName.Text)
                .DisposeWith(disposables);
        });
    }
}