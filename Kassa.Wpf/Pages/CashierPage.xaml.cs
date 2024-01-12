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
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для CashierPage.xaml
/// </summary>
public partial class CashierPage : ReactiveUserControl<CashierVm>
{
    public CashierPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, x => x.FastAddictives, x => x.FastAddictives.ItemsSource)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.GoBackCommand, x => x.NavigateBackButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.CurrentProductViewModels, x => x.ProductsHost.ItemsSource)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.ProductsHost.ItemsSource)
                .Subscribe(x =>
                {

                });

            this.OneWayBind(ViewModel, x => x.ShoppingList.AddictiveViewModels, x => x.ShoppingListItems.ItemsSource)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList.IncreaseCommand, x => x.IncreaseButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList.DecreaseCommand, x => x.DecreaseButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ShoppingList.Subtotal, x => x.SubtotalCost.Text, x => $"{x} ₽")
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList.RemoveCommand, x => x.RemoveButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.CreateTotalCommentCommand, x => x.TotalCommentButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SearchAddictiveCommand, x => x.SearchAddictivesButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.ShoppingList.IsMultiSelect, x => x.MultiSelectCheckbox.IsChecked)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectCategoryCommand, x => x.AllCategories)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectCategoryCommand, x => x.FastFoodCategory)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectCategoryCommand, x => x.DrinksCategory)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectCategoryCommand, x => x.FavoriteCategory1)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectCategoryCommand, x => x.FavoriteCategory2)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectCategoryCommand, x => x.FavoriteCategory3)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SearchProductCommand, x => x.SearchFoodButton)
                .DisposeWith(disposables);

        });
    }
}
