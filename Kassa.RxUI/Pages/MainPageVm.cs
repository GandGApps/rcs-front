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
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using static System.Formats.Asn1.AsnWriter;

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

    private readonly ICashierService _cashierService;
    private readonly IShiftService _shiftService;
    private readonly IIngridientsService _ingridientsService;

    public MainPageVm(ICashierService cashierService,IShiftService shiftService, IIngridientsService ingridientsService)
    {
        _cashierService = cashierService;
        _shiftService = shiftService;
        _ingridientsService = ingridientsService;

        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new TurnOffDialogViewModel(_shiftService)).FirstAsync();
        });

        OpenProfileDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!await TryAuthorizePageAccess<PersonalPageVm>(_shiftService))
            {
                return;
            }

            await MainViewModel.ShowDialogAsync(new ProfileDialogViewModel(_shiftService));
        });

        OpenDocumnetsDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!await TryAuthorizePageAccess<PageViewModel>(_shiftService))
            {
                return;
            }

            await MainViewModel.ShowDialogAsync(new DocumentsDialogViewModel());
        });

        OpenPersonnelDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!await TryAuthorizePageAccess<PageViewModel>(_shiftService))
            {
                return;
            }

            await MainViewModel.ShowDialogAsync(new PersonnelDialogViewModel());
        });

        OpenDeliviryDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!await TryAuthorizePageAccess<AllDeliveriesPageVm>(_shiftService))
            {
                return;
            }

            await MainViewModel.GoToPage(RcsKassa.CreateAndInject<AllDeliveriesPageVm>());
        });

        OpenServicesDialog = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка данных...";

            if (!await TryAuthorizePageAccess<ServicePageVm>(_shiftService))
            {
                return;
            }

            await MainViewModel.GoToPage(RcsKassa.CreateAndInject<ServicePageVm>());
        });

        GoToCashier = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка данных...";

            var order = await _cashierService.CreateOrder(false);
            var scope = _ingridientsService.CreateStorageScope();

            if (!await TryAuthorizePageAccess<OrderEditPageVm>(shiftService))
            {
                return;
            }

            await cashierService.SelectCurrentOrder(order);

            await MainViewModel.GoToPage<OrderEditPageVm>(order, scope);
        });

        OpenCurrentOrderCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка данных...";

            var order = _cashierService.CurrentOrder;
            var scope = _ingridientsService.CreateStorageScope();

            if (!await TryAuthorizePageAccess<OrderEditPageVm>(shiftService))
            {
                return;
            }

            if (order == null)
            {
                await MainViewModel.OkMessageAsync("Нет текущего заказа", "JustFailed");
                return;
            }

            await MainViewModel.GoToPage<OrderEditPageVm>(order, scope);
        });

        CurrentShiftMemberName = null!;
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

    protected override void Initialize(CompositeDisposable disposables)
    {
        _shiftService.CurrentShift.Subscribe(shift =>
        {
            if (shift == null)
            {

                CurrentShiftMemberName = "Смена не открыта";
            }
            else
            {
                var dto = shift.CreateDto();
                CurrentShiftMemberName = shift.Member.Name;
                CurrentShiftOpennedDate = dto.Start;
            }
        }).DisposeWith(disposables);
    }

}
