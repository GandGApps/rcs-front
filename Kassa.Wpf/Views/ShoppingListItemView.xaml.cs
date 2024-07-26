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
    public static readonly ReactiveCommand<IShoppingListItemVm, Unit> UpdateSelectionShoppingListItemCommand = ReactiveCommand.CreateFromTask<IShoppingListItemVm>(async vm =>
    {
        vm.IsSelected = !vm.IsSelected;
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
