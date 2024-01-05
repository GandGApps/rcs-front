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
using Kassa.RxUI;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Interaction logic for AddictiveView.xaml
/// </summary>
public partial class AddictiveView : ReactiveUserControl<AddictiveViewModel>
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
            DataContext = ViewModel;

            this.OneWayBind(ViewModel, vm => vm.AddToShoppingListCommand, v => v.Command)
                .DisposeWith(disposabels);
        });
    }
}
[EditorBrowsable(EditorBrowsableState.Never)]
public class DesignerAddictiveViewModel : AddictiveViewModel
{
    public DesignerAddictiveViewModel() : base()
    {
    }
}