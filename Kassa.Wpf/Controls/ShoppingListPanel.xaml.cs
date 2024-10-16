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
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using Kassa.Shared;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf.Controls;
/// <summary>
/// Interaction logic for ShoppingListPanel.xaml
/// </summary>
public sealed partial class ShoppingListPanel : UserControl
{
    public static readonly DependencyProperty OrderEditVmProperty =
        DependencyProperty.Register(nameof(OrderEditVm), typeof(IOrderEditVm), typeof(ShoppingListPanel), new PropertyMetadata(OnOrderEditServiceChanged));


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

            if (e.NewValue is IOrderEditVm orderEditVm)
            {
                orderEditVm
                    .WhenAnyValue(x => x.IsForHere)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isForHere =>
                    {
                        panel.IsForHereText.Text = isForHere ? "Здесь" : "С собой";
                    })
                    .DisposeWith(panel._disposables);

                panel.TimeWhenStart.Text = orderEditVm.WhenOrderStarted.ToString("dd.MM  HH:mm");

                orderEditVm.ShoppingList
                    .WhenAnyValue(x => x.IsMultiSelect)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isMultiSelect =>
                    {
                        panel.MultiSelectCheckbox.IsChecked = isMultiSelect;
                    })
                    .DisposeWith(panel._disposables);

                panel.OrderId.Text = orderEditVm.OrderId.ToString("N")[..2];
            }
        }
    }

    private CompositeDisposable? _disposables;

    public ShoppingListPanel()
    {
        InitializeComponent();

        var shiftService = RcsKassa.GetRequiredService<IShiftService>();
        var currentShift = shiftService.CurrentShift.Value;

        Shift = currentShift;
    }

    public IOrderEditVm? OrderEditVm
    {
        get => (IOrderEditVm?)GetValue(OrderEditVmProperty);
        set => SetValue(OrderEditVmProperty, value);
    }

    public IShift? Shift
    {
        get => (IShift?)GetValue(ShiftProperty);
        set => SetValue(ShiftProperty, value);
    }

    private void IsForHereButtonClick(object sender, RoutedEventArgs e)
    {
        if (OrderEditVm == null)
        {
            return;
        }

        OrderEditVm.IsForHere = !OrderEditVm.IsForHere;
    }

    private void MultiSelectCheckboxClick(object sender, RoutedEventArgs e)
    {
        if (OrderEditVm is null)
        {
            return;
        }

        OrderEditVm.ShoppingList.IsMultiSelect = !OrderEditVm.ShoppingList.IsMultiSelect;
    }
}
