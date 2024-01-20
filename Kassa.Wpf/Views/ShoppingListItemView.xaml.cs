using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
using Kassa.RxUI;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для ShoppingListItemView.xaml
/// </summary>
public partial class ShoppingListItemView : ReactiveUserControl<ShoppingListItemDto>
{
    public static readonly ReactiveCommand<IShoppingListItemDto, Unit> SelectShoppingListItemCommand = ReactiveCommand.CreateFromTask<IShoppingListItemDto>(async shoppingListItem =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();

        await cashierService.SelectShoppingListItem(shoppingListItem);
    });

    public static readonly ReactiveCommand<IShoppingListItemDto, Unit> UnselectShoppingListItemCommand = ReactiveCommand.CreateFromTask<IShoppingListItemDto>(async shoppingListItem =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();

        await cashierService.UnselectShoppingListItem(shoppingListItem);
    });

    public ShoppingListItemView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            ProductCheckbox.Command = ViewModel!.IsSelected ? UnselectShoppingListItemCommand : SelectShoppingListItemCommand;

            ProductCheckbox.CommandParameter = ViewModel;
        });
    }
}
