using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public class MainPageVm : PageViewModel
{
    public ReactiveCommand<Unit, Unit> CloseCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenProfileDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenDocumnetsDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenDeliviryDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenPersonnelDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenServicesDialog
    {
        get;
    }

    public ICommand GoToCashier
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenCurrentOrderCommand
    {
        get;
    }

    public MainPageVm()
    {
        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new TurnOffDialogViewModel()).FirstAsync();
        });

        OpenProfileDialog = CreateOpenDialogCommand(() => new ProfileDialogViewModel());
        OpenDocumnetsDialog = CreateOpenDialogCommand(() => new DocumentsDialogViewModel());
        OpenDeliviryDialog = CreateOpenDialogCommand(() => new DeliviryDialogViewModel());
        OpenPersonnelDialog = CreateOpenDialogCommand(() => new PersonnelDialogViewModel());
        OpenServicesDialog = CreateOpenDialogCommand(() => new ServicesDialogViewModel());

        GoToCashier = ReactiveCommand.CreateFromTask(async () =>
        {
            var cashierService = await Locator.GetInitializedService<ICashierService>();
            var order = await cashierService.CreateOrder();

            await cashierService.SelectCurrentOrder(order);

            await MainViewModel.GoToPageCommand.Execute(new OrderVm(order, cashierService)).FirstAsync();
        });

        OpenCurrentOrderCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var cashierService = await Locator.GetInitializedService<ICashierService>();
            var order = cashierService.CurrentOrder;

            if (order == null)
            {
                await MainViewModel.OkMessage("Нет текущего заказа", "JustFailed");
                return;
            }

            await MainViewModel.GoToPageCommand.Execute(new OrderVm(order, cashierService)).FirstAsync();
        });
    }

    private ReactiveCommand<Unit, Unit> CreateOpenDialogCommand(Func<DialogViewModel> dialogViewModel) => ReactiveCommand.CreateFromTask(async () =>
    {
        await MainViewModel!.DialogOpenCommand.Execute(dialogViewModel()).FirstAsync();
    });
}
