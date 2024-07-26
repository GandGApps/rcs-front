using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Kassa.RxUI;
using ReactiveUI;

namespace Kassa.Wpf.Controls;

/// <summary>
/// Interaction logic for ShoppingList.xaml
/// </summary>
public sealed partial class ShoppingList : UserControl
{

    public static readonly DependencyProperty ShoppinngListVmProperty =
        DependencyProperty.Register(nameof(ShoppinngListVm), typeof(ShoppingListViewModel), typeof(ShoppingList), new PropertyMetadata(OrderEditServiceChanged));

    public static readonly DependencyPropertyKey ShoppingListItemsProperty =
        DependencyProperty.RegisterReadOnly(nameof(ShoppingListItems), typeof(ReadOnlyCollection<ProductShoppingListItemViewModel>), typeof(ShoppingList), new PropertyMetadata());

    public static void OrderEditServiceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is ShoppingList shoppingList)
        {
            shoppingList._disposables?.Dispose();
            shoppingList._disposables = [];

            if (e.NewValue is ShoppingListViewModel shoppingListVm)
            {
                shoppingList.ShoppingListItems = shoppingListVm.ProductShoppingListItems;
                shoppingList.Items.ItemsSource = shoppingListVm.ProductShoppingListItems;

                shoppingListVm.ProductShoppingListItems.WhenAnyValue(x => x.Count)
                     .Buffer(2, 1)
                     .Subscribe(x =>
                     {
                         if (x[0] < x[1])
                         {
                             shoppingList.ScrollViewerForShoppingListItems.ScrollToEnd();
                         }
                     })
                     .DisposeWith(shoppingList._disposables);
            }
        }
    }

    private CompositeDisposable? _disposables;

    public ShoppingList()
    {
        InitializeComponent();
    }

    public ShoppingListViewModel? ShoppinngListVm
    {
        get => (ShoppingListViewModel?)GetValue(ShoppinngListVmProperty);
        set => SetValue(ShoppinngListVmProperty, value);
    }

    public ReadOnlyCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get => (ReadOnlyCollection<ProductShoppingListItemViewModel>?)GetValue(ShoppingListItemsProperty.DependencyProperty);
        set => SetValue(ShoppingListItemsProperty, value);
    }

}
