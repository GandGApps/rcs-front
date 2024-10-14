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
using Kassa.Shared.ServiceLocator;
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

    private readonly ICashierShiftService _cashierShiftService;
    private readonly IShiftService _shiftService;

    public MainPageVm(ICashierShiftService cashierShiftService, IShiftService shiftService)
    {
        _cashierShiftService = cashierShiftService;
        _shiftService = shiftService;

        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new TurnOffDialogViewModel()).FirstAsync();
        });

        OpenProfileDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!await TryAuthorizePageAccess<PersonalPageVm>(_shiftService))
            {
                return;
            }

            await MainViewModel.ShowDialogAsync(new ProfileDialogViewModel());
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

            await MainViewModel.GoToPage(new AllDeliveriesPageVm());
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

            var cashierService = RcsLocator.Scoped.GetRequiredService<ICashierService>();
            var order = await cashierService.CreateOrder(false);
            var additiveService = RcsLocator.Scoped.GetRequiredService<IAdditiveService>();
            var productServise = RcsLocator.Scoped.GetRequiredService<IProductService>();
            var ingredientService = RcsLocator.Scoped.GetRequiredService<IIngridientsService>();
            var shiftService = RcsLocator.GetRequiredService<IShiftService>();
            var categoryService = RcsLocator.Scoped.GetRequiredService<ICategoryService>();
            var receiptService = RcsLocator.Scoped.GetRequiredService<IReceiptService>();
            var scope = ingredientService.CreateStorageScope();

            if (!await TryAuthorizePageAccess<OrderEditPageVm>(shiftService))
            {
                return;
            }

            await cashierService.SelectCurrentOrder(order);


            var orderEditPageVm = new OrderEditPageVm(order, scope, cashierService, additiveService, productServise, categoryService, receiptService, ingredientService);

            await MainViewModel.GoToPageCommand.Execute(orderEditPageVm).FirstAsync();
        });

        OpenCurrentOrderCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка данных...";

            var cashierService = RcsLocator.Scoped.GetRequiredService<ICashierService>();
            var additiveService = RcsLocator.Scoped.GetRequiredService<IAdditiveService>();
            var order = cashierService.CurrentOrder;
            var productServise = RcsLocator.Scoped.GetRequiredService<IProductService>();
            var ingredientService = RcsLocator.Scoped.GetRequiredService<IIngridientsService>();
            var shiftService = RcsLocator.GetRequiredService<IShiftService>();
            var categoryService = RcsLocator.Scoped.GetRequiredService<ICategoryService>();
            var receiptService = RcsLocator.Scoped.GetRequiredService<IReceiptService>();
            var scope = ingredientService.CreateStorageScope();

            if (!await TryAuthorizePageAccess<OrderEditPageVm>(shiftService))
            {
                return;
            }

            if (order == null)
            {
                await MainViewModel.OkMessageAsync("Нет текущего заказа", "JustFailed");
                return;
            }

            var orderEditPageVm = new OrderEditPageVm(order, scope, cashierService, additiveService, productServise, categoryService, receiptService, ingredientService);

            await MainViewModel.GoToPageCommand.Execute(orderEditPageVm).FirstAsync();
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
        var shiftService = RcsLocator.GetRequiredService<IShiftService>();

        shiftService.CurrentShift.Subscribe(async shift =>
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
