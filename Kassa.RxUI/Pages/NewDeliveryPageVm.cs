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
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class NewDeliveryPageVm : PageViewModel
{
    private IOrderEditService _orderEdit = null!;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;

    public NewDeliveryPageVm(ICashierService cashierService, IAdditiveService additiveService) : this(cashierService, additiveService, null)
    {
        IsNewClient = true;
    }

    public NewDeliveryPageVm(ICashierService cashierService, IAdditiveService additiveService, ClientViewModel? clientViewModel)
    {
        _cashierService = cashierService;
        _additiveService = additiveService;

        DeliveryId = Guid.NewGuid();
        Client = clientViewModel;

        Phone = Client?.Phone ?? string.Empty;
        NameWithMiddleName = $"{clientViewModel?.FirstName} {clientViewModel?.MiddleName}";
        Address = Client?.Address ?? string.Empty;
        House = Client?.House ?? string.Empty;
        Building = Client?.Building ?? string.Empty;
        Entrance = Client?.Entrance ?? string.Empty;
        Floor = Client?.Floor ?? string.Empty;
        Apartment = Client?.Apartment ?? string.Empty;
        Intercom = Client?.Intercom ?? string.Empty;
        Card = Client?.Card ?? string.Empty;
        AddressNote = Client?.AddressNote ?? string.Empty;
        LastName = Client?.LastName ?? string.Empty;
        FirstName = Client?.FirstName ?? string.Empty;
        MiddleName = Client?.MiddleName ?? string.Empty;
        Miscellaneous = Client?.Miscellaneous ?? string.Empty;

        this.WhenAnyValue(x => x.IsPickup, x => x.IsDelivery, (isPickup, isDelivery) => isPickup ? "Самовывоз" : isDelivery ? "Доставка курьером" : string.Empty)
            .ToPropertyEx(this, x => x.TypeOfOrder);

        SelectDistrictAndStreetCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var districtService = await Locator.GetInitializedService<IDistrictService>();

            var districtDialog = new AllDistrictsDialogViewModel(districtService);

            await MainViewModel.ShowDialogAndWaitClose(districtDialog);

            if (districtDialog.SelectedItem is null)
            {
                return;
            }

            var streetService = await Locator.GetInitializedService<IStreetService>();

            var streetDialog = new StreetsDialogViewModel(districtDialog.SelectedItem, streetService);

            await MainViewModel.ShowDialogAndWaitClose(streetDialog);

            if (streetDialog.SelectedItem is null)
            {
                return;
            }

            District = districtDialog.SelectedItem;
            Street = streetDialog.SelectedItem;
        }).DisposeWith(InternalDisposables);

        SwitchOrderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!IsOrderEditOpened)
            {
                await MainViewModel.Router.NavigateBack.Execute().FirstAsync();
                return;
            }

            await MainViewModel.Router.Navigate.Execute(OrderEditPageVm).FirstAsync();

        }).DisposeWith(InternalDisposables);

        BackButtonCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            while (MainViewModel.Router.NavigationStack[^1] is not AllDeliveriesPageVm)
            {
                await MainViewModel.GoBackCommand.Execute().FirstAsync();
            }

        }).DisposeWith(InternalDisposables);

        SwitchToPaymentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!IsPaymentPageOpenned)
            {
                await MainViewModel.Router.NavigateBack.Execute().FirstAsync();
                return;
            }
            await MainViewModel.Router.Navigate.Execute(PaymentPageVm).FirstAsync();

        }).DisposeWith(InternalDisposables);

        WriteProblemCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var problemDialog = new ProblemDialogViewModel
            {
                Problem = Problem,
                IsProblematicDelivery = IsProblematicDelivery
            };

            problemDialog.OkCommand.Subscribe( _ =>
            {
                Problem = problemDialog.Problem;
                IsProblematicDelivery = problemDialog.IsProblematicDelivery;
            });

            await MainViewModel.ShowDialogAndWaitClose(problemDialog);

        }).DisposeWith(InternalDisposables);
    }

    public new CompositeDisposable InternalDisposables => base.InternalDisposables;

    public Guid DeliveryId
    {
        get; set;
    }

    [Reactive]
    public bool IsOrderEditOpened
    {
        get; set;
    }

    [Reactive]
    public bool IsPickup
    {
        get; set;
    }

    [Reactive]
    public bool IsDelivery
    {
        get; set;
    }

    public ClientViewModel? Client
    {
        get;
    }

    [Reactive]
    public string NameWithMiddleName
    {
        get; set;
    }

    [Reactive]
    public string House
    {
        get; set;
    }

    [Reactive]
    public string Building
    {
        get; set;
    }

    [Reactive]
    public string Entrance
    {
        get; set;
    }

    [Reactive]
    public string Floor
    {
        get; set;
    }
    [Reactive]
    public string Apartment
    {
        get; set;
    }
    [Reactive]
    public string Intercom
    {
        get; set;
    }

    [Reactive]
    public string Phone
    {
        get;
        set;
    }

    [Reactive]
    public string Card
    {
        get; set;
    }


    [Reactive]
    public string Address
    {
        get; set;
    }

    [Reactive]
    public string? Comment
    {
        get; set;
    }

    [Reactive]
    public string AddressNote
    {
        get; set;
    }

    [Reactive]
    public string LastName
    {
        get; set;
    }

    [Reactive]
    public string FirstName
    {
        get; set;
    }

    [Reactive]
    public string MiddleName
    {
        get; set;
    }

    public extern string TypeOfOrder
    {
        [ObservableAsProperty]
        get;
    }

    public extern string FullName
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public string Miscellaneous
    {
        get; set;
    }

    [Reactive]
    public StreetViewModel? Street
    {
        get; set;
    }

    [Reactive]
    public DistrictViewModel? District
    {
        get; set;
    }

    public bool IsNewClient
    {
        get;
    }

    [Reactive]
    public bool IsOutOfTurn
    {
        get; set;
    }

    [Reactive]
    public bool IsPaymentPageOpenned
    {
        get; set;
    }

    [Reactive]
    public string Problem
    {
        get; set;
    } = string.Empty;

    [Reactive]
    public bool IsProblematicDelivery
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> SelectDistrictAndStreetCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SwitchOrderCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SwitchToPaymentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> BackButtonCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> WriteProblemCommand
    {
        get;
    }

    [Reactive]
    public NewDeliveryOrderEditPageVm OrderEditPageVm
    {
        get; set;
    } = null!;

    [Reactive]
    public DeliveryPaymentPageVm PaymentPageVm
    {
        get; set;
    } = null!;

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var cashierService = await Locator.GetInitializedService<ICashierService>();

        _orderEdit = await cashierService.CreateOrder(true);
        await cashierService.SelectCurrentOrder(_orderEdit);

        OrderEditPageVm = new NewDeliveryOrderEditPageVm(_orderEdit, _cashierService, _additiveService);

        var payment = await cashierService.CreatePayment(_orderEdit);
        PaymentPageVm = new(payment);

        await OrderEditPageVm.InitializeAsync();
        await PaymentPageVm.InitializeAsync();

        Disposable.Create(() =>
        {
            _orderEdit.Dispose();
            OrderEditPageVm.Dispose();

            payment.Dispose();
            PaymentPageVm.Dispose();
        }).DisposeWith(disposables);

    }
}
