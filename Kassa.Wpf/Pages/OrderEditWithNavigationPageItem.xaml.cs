using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Interaction logic for OrderEditWithNavigationPageItem.xaml
/// </summary>
public sealed partial class OrderEditWithNavigationPageItem : ReactiveUserControl<OrderEditWithNavigationPageItemVm>
{
    public OrderEditWithNavigationPageItem()
    {
        InitializeComponent();

        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            Debug.Assert(ViewModel != null);

            NavigateBackButton.Command = ViewModel.NavigateBackCategoryCommand;

            this.OneWayBind(ViewModel, x => x.FastAdditives, x => x.FastAddictives.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.CurrentHostedItems, x => x.ProductsHost.ItemsSource)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList!.IncreaseSelectedCommand, x => x.IncreaseButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList!.DecreaseSelectedCommand, x => x.DecreaseButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ShoppingList!.Subtotal, x => x.SubtotalCost.Text, x => $"{x.ToString("0.##", QuantityVolumeDialogVewModel.RuCultureInfo)} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ShoppingList!.Total, x => x.TotalCost.Text, x => $"{x.ToString("0.##", QuantityVolumeDialogVewModel.RuCultureInfo)} ₽")
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.ShoppingList!.RemoveSelectedCommand, x => x.RemoveButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.CreateTotalCommentCommand, x => x.TotalCommentButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SearchAddictiveCommand, x => x.SearchAddictivesButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenPortionDialogCommand, x => x.PortionButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.SelectRootCategoryCommand, x => x.AllCategories)
                .DisposeWith(disposables);

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

            this.BindCommand(ViewModel, x => x.OpenQuantityVolumeDialogCommand, x => x.QuantityVolumeButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.GoToAllDeliveriesPageCommand, x => x.GoToDeliveryButton)
                .DisposeWith(disposables);

            ShoppingListPanel.OrderEditVm = ViewModel;
            ShoppingListItems.ShoppinngListVm = ViewModel.ShoppingList;
        });
    }
}
