using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using Kassa.BuisnessLogic.Services;
using ReactiveUI;

namespace Kassa.Wpf.Controls;
/// <summary>
/// Interaction logic for ShoppingListPanel.xaml
/// </summary>
public sealed partial class ShoppingListPanel : UserControl
{
    public static readonly DependencyProperty OrderEditServiceProperty =
        DependencyProperty.Register(nameof(OrderEditService), typeof(IOrderEditService), typeof(ShoppingListPanel), new PropertyMetadata(OnOrderEditServiceChanged));


    public static readonly DependencyProperty ShiftProperty =
        DependencyProperty.Register(nameof(Shift), typeof(IShift), typeof(ShoppingListPanel), new PropertyMetadata(OnShiftChanged));

    private static void OnShiftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShoppingListPanel panel)
        {
            if (e.NewValue is IShift shift)
            {
                panel.CashierName.Text = shift.Member.Name;
            }
        }
    }

    private static void OnOrderEditServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

        if (d is ShoppingListPanel panel)
        {
            panel._disposables?.Dispose();
            panel._disposables = [];

            if (e.NewValue is IOrderEditService orderEditService)
            {
                orderEditService.IsForHere
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isForHere =>
                    {
                        panel.IsForHereText.Text = isForHere ? "Здесь" : "С собой";
                    })
                    .DisposeWith(panel._disposables);

                panel.TimeWhenStart.Text = orderEditService.WhenOrderStarted.ToString("dd.MM  HH:mm");

                orderEditService.IsMultiSelect
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isMultiSelect =>
                    {
                        panel.MultiSelectCheckbox.IsChecked = isMultiSelect;
                    })
                    .DisposeWith(panel._disposables);

            }
        }
    }

    private CompositeDisposable? _disposables;

    public ShoppingListPanel()
    {
        InitializeComponent();
    }

    public IOrderEditService? OrderEditService
    {
        get => (IOrderEditService?)GetValue(OrderEditServiceProperty);
        set => SetValue(OrderEditServiceProperty, value);
    }

    public IShift? Shift
    {
        get => (IShift?)GetValue(ShiftProperty);
        set => SetValue(ShiftProperty, value);
    }

    private void IsForHereButtonClick(object sender, RoutedEventArgs e)
    {
        if (OrderEditService == null)
        {
            return;
        }

        var isForHere = OrderEditService.IsForHere.Value;

        OrderEditService.SetIsForHere(!isForHere);
    }

    private void MultiSelectCheckboxClick(object sender, RoutedEventArgs e)
    {
        if (OrderEditService is null)
        {
            return;
        }

        var isMultiSelect = OrderEditService.IsMultiSelect.Value;

        OrderEditService.SetMultiSelect(!isMultiSelect);
    }
}
