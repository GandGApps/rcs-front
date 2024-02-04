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
    public static readonly ReactiveCommand<ProductShoppingListItemViewModel, Unit> UpdateSelectionShoppingListItemCommand = ReactiveCommand.CreateFromTask<ProductShoppingListItemViewModel>(async vm =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();

        if (vm.IsSelected)
        {
            await cashierService.UnselectShoppingListItem(vm.Source);
        }
        else
        {
            await cashierService.SelectShoppingListItem(vm.Source);
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
