using System.Reactive;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для ShoppingListItemView.xaml
/// </summary>
public partial class ShoppingListItemView : ReactiveUserControl<ProductShoppingListItemViewModel>
{
    public static readonly ReactiveCommand<IShoppingListItem, Unit> UpdateSelectionShoppingListItemCommand = ReactiveCommand.CreateFromTask<IShoppingListItem>(async vm =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();


        if (vm.IsSelected)
        {
            await cashierService.UnselectShoppingListItem(vm.SourceDto);
        }
        else
        {
            await cashierService.SelectShoppingListItem(vm.SourceDto);
        }
    });



    public ShoppingListItemView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            ProductCheckbox.Command = UpdateSelectionShoppingListItemCommand;

            ProductCheckbox.CommandParameter = ViewModel;
        });
    }
}
