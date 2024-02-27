using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using Kassa.RxUI;
using Kassa.RxUI.Pages;
using Kassa.Wpf.Themes;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для OrderPage.xaml
/// </summary>
public partial class OrderPage : ReactiveUserControl<OrderVm>
{
    public OrderPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            NavigateBackButton.Command = CategoryViewModel.NavigateBackCategoryCommand;

            this.OneWayBind(ViewModel, x => x.FastAdditives, x => x.FastAddictives.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.CurrentCategoryItems, x => x.ProductsHost.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ShoppingListItems, x => x.ShoppingListItems.ItemsSource)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList.IncreaseCommand, x => x.IncreaseButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList.DecreaseCommand, x => x.DecreaseButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ShoppingList.Subtotal, x => x.SubtotalCost.Text, x => $"{x} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ShoppingList.Total, x => x.TotalCost.Text, x => $"{x} ₽")
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList.RemoveCommand, x => x.RemoveButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.CreateTotalCommentCommand, x => x.TotalCommentButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SearchAddictiveCommand, x => x.SearchAddictivesButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.IsMultiSelect, x => x.MultiSelectCheckbox.IsChecked)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectRootCategoryCommand, x => x.AllCategories)
                .DisposeWith(disposables);

            /*this.BindCommand(ViewModel, x => x.SelectFavouriteCommand, x => x.FastFoodCategory)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectFavouriteCommand, x => x.DrinksCategory)
                .DisposeWith(disposables);*/

            this.BindCommand(ViewModel, x => x.SelectFavouriteCommand, x => x.FavoriteCategory1)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectFavouriteCommand, x => x.FavoriteCategory2)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectFavouriteCommand, x => x.FavoriteCategory3)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SearchProductCommand, x => x.SearchFoodButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenMoreDialogCommand, x => x.MoreButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenDiscountsAndSurchargesDialog, x => x.PricingDetailsButton)
                .DisposeWith(disposables);

            ViewModel.WhenAnyValue(x => x.ShoppingListItems.Count)
                     .Buffer(2, 1)
                     .Subscribe(x =>
                     {
                         if (x[0] < x[1])
                         {
                             ScrollViewerForShoppingListItems.ScrollToEnd();
                         }
                     })
                     .DisposeWith(disposables);
        });
    }

    private void OpenColorChanger(object sender, RoutedEventArgs e)
    {
        var theme = App.GetCurrentThemeName();

        if (theme == Theme.Light)
        {
            App.SwitchTheme(Theme.PornhubTheme);
        }
        else
        {
            App.SwitchTheme(Theme.Light);
        }
    }
}
