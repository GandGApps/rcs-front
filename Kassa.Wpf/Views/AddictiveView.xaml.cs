using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
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
using Kassa.DataAccess;
using Kassa.RxUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Wpf.Views;
/// <summary>
/// Interaction logic for AddictiveView.xaml
/// </summary>
public partial class AddictiveView : ReactiveUserControl<Additive>
{

    public static readonly DependencyProperty HasAddictiveIconProperty = DependencyProperty.Register(
               nameof(HasAddictiveIcon), typeof(bool), typeof(AddictiveView), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
               nameof(Command), typeof(ICommand), typeof(AddictiveView), new PropertyMetadata(default(ICommand)));

    public bool HasAddictiveIcon
    {
        get => (bool)GetValue(HasAddictiveIconProperty);
        set => SetValue(HasAddictiveIconProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public AddictiveView()
    {
        InitializeComponent();

        this.WhenActivated(disposabels =>
        {
            DataContext = new AdditiveViewModel(ViewModel!);
        });
    }
}
[EditorBrowsable(EditorBrowsableState.Never)]
public class DesignerAddictiveViewModel
{

    [Reactive]
    public string Name
    {
        get; set;
    }

    [Reactive]
    public string СurrencySymbol
    {
        get; set;
    }

    [Reactive]
    public double Price
    {
        get; set;
    }

    [Reactive]
    public bool IsAdded
    {
        get; set;
    }

    [Reactive]
    public double Count
    {
        get; set;
    }

    [Reactive]
    public string Measure
    {
        get; set;
    }


    [Reactive]
    public bool IsAvailable
    {
        get; set;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> AddToShoppingListCommand
    {
        get; set;
    }
}
