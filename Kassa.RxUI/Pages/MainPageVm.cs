using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

        OpenProfileDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (!await TryAuthorizePageAccess<PersonalPageVm>(shiftService))
            {

                return;
            }

            await MainViewModel.ShowDialog(new ProfileDialogViewModel());
        });

        OpenDocumnetsDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (!await TryAuthorizePageAccess<PageViewModel>(shiftService))
            {
                return;
            }

            await MainViewModel.ShowDialog(new DocumentsDialogViewModel());
        });

        OpenPersonnelDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (!await TryAuthorizePageAccess<PageViewModel>(shiftService))
            {
                return;
            }

            await MainViewModel.ShowDialog(new PersonnelDialogViewModel());
        });

        OpenDeliviryDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (!await TryAuthorizePageAccess<AllDeliveriesPageVm>(shiftService))
            {
                return;
            }

            await MainViewModel.GoToPage(new AllDeliveriesPageVm());
        });

        OpenServicesDialog = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка данных...";

            var cashierService = await Locator.GetInitializedService<ICashierService>();
            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (!await TryAuthorizePageAccess<ServicePageVm>(shiftService))
            {
                return;
            }

            await MainViewModel.GoToPage(new ServicePageVm(cashierService, shiftService));
        });

        GoToCashier = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка данных...";

            var cashierService = await Locator.GetInitializedService<ICashierService>();
            var order = await cashierService.CreateOrder(false);
            var additveService = await Locator.GetInitializedService<IAdditiveService>();

            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (!await TryAuthorizePageAccess<OrderEditPageVm>(shiftService))
            {
                return;
            }

            await cashierService.SelectCurrentOrder(order);

            await MainViewModel.GoToPageCommand.Execute(new OrderEditPageVm(order, cashierService, additveService)).FirstAsync();
        });

        OpenCurrentOrderCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка данных...";

            var cashierService = await Locator.GetInitializedService<ICashierService>();
            var additiveService = await Locator.GetInitializedService<IAdditiveService>();
            var order = cashierService.CurrentOrder;

            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (!await TryAuthorizePageAccess<OrderEditPageVm>(shiftService))
            {
                return;
            }

            if (order == null)
            {
                await MainViewModel.OkMessage("Нет текущего заказа", "JustFailed");
                return;
            }

            await MainViewModel.GoToPageCommand.Execute(new OrderEditPageVm(order, cashierService, additiveService)).FirstAsync();
        });
    }

    [Reactive]
    public string CurrentShiftMemberName
    {
        get; set;
    }

    [Reactive]
    public DateTime? CurrentShiftOpennedDate
    {
        get; set;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var shiftService = await Locator.GetInitializedService<IShiftService>();

        shiftService.CurrentShift.Subscribe(async shift =>
        {
            if (shift == null)
            {

                CurrentShiftMemberName = "Смена не открыта";
            }
            else
            {
                var dto = await shift.CreateDto();
                CurrentShiftMemberName = shift.Member.Name;
                CurrentShiftOpennedDate = dto.Start;
            }
        }).DisposeWith(disposables);

        Disposable.Create(() =>
        {
        });
    }

}
