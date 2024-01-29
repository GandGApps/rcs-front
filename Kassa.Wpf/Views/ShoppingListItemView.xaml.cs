using System.Reactive;
using Kassa.BuisnessLogic;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для ShoppingListItemView.xaml
/// </summary>
public partial class ShoppingListItemView : ReactiveUserControl<ProductShoppingListItemDto>
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
