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
    private IPaymentService _paymentService = null!;
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
            .ToPropertyEx(this, x => x.TypeOfOrder)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.CourierViewModel, (CourierViewModel? x) => x is not null ? x.FullName : string.Empty)
            .ToPropertyEx(this, x => x.CourierFullName)
            .DisposeWith(InternalDisposables);

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

            problemDialog.OkCommand.Subscribe(_ =>
            {
                Problem = problemDialog.Problem;
                IsProblematicDelivery = problemDialog.IsProblematicDelivery;
            });

            await MainViewModel.ShowDialogAndWaitClose(problemDialog);

        }).DisposeWith(InternalDisposables);

        SelectCourierCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var courierService = await Locator.GetInitializedService<ICourierService>();
            var dialog = new SearchCourierDialogViewModel(courierService);

            dialog.OkCommand.Subscribe(x =>
            {
                CourierViewModel = x;
            });

            await MainViewModel.ShowDialogAndWaitClose(dialog);

        }).DisposeWith(InternalDisposables);

        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!IsPickup && CourierViewModel is null)
            {
                await MainViewModel.OkMessage("Не выбран курьер");
                return;
            }

            if (!IsDelivery && (District is null || Street is null))
            {
                await MainViewModel.OkMessage("Не выбран район или улица");
                return;
            }

            if (!PaymentPageVm.IsExactAmount)
            {
                await MainViewModel.OkMessage("Необходимо ввести сумму оплаты");
                return;
            }

            var order = await _orderEdit.GetOrder();

            if (!order.Products.Any())
            {
                await MainViewModel.OkMessage("Не выбраны товары");
                return;
            }

            if (!IsAllClientInfoFilled())
            {
                await MainViewModel.OkMessage("Не все данные клиента заполнены");
                return;
            }

            var separetedNameWithMiddleName = NameWithMiddleName.Split(' ', 1);
            if (separetedNameWithMiddleName.Length == 1)
            {
                separetedNameWithMiddleName = [separetedNameWithMiddleName[0], string.Empty];
            }
            else if (separetedNameWithMiddleName.Length > 2)
            {
                separetedNameWithMiddleName = [separetedNameWithMiddleName[0], separetedNameWithMiddleName[1]];
            }
            else
            {
                separetedNameWithMiddleName = [string.Empty, string.Empty];
            }

            if (IsNewClient)
            {

                if (!IsAllAddressInfoFilled())
                {
                    await MainViewModel.OkMessage("Не все данные адреса заполнены. Они обязательны для нового клиента.");
                    return;
                }

                var client = new ClientDto
                {
                    Id = Guid.NewGuid(),
                    Address = Address,
                    AddressNote = AddressNote,
                    Apartment = Apartment,
                    Building = Building,
                    Card = Card,
                    StreetId = Street!.Id,
                    Entrance = Entrance,
                    Floor = Floor,
                    House = House,
                    Intercom = Intercom,
                    LastName = LastName,
                    FirstName = separetedNameWithMiddleName[0],
                    MiddleName = separetedNameWithMiddleName[1],
                    Miscellaneous = Miscellaneous,
                    Phone = Phone
                };

                var clientService = await Locator.GetInitializedService<IClientService>();
                await clientService.AddClient(client);
            }

            var loading = MainViewModel.ShowLoadingDialog("Сохранение заказа");

            order.AddressNote = AddressNote;
            order.Comment = Comment;
            order.Apartment = Apartment;
            order.Building = Building;
            order.Card = Card;
            order.Id = DeliveryId;
            order.IsDelivery = IsDelivery;
            order.IsPickup = IsPickup;
            order.DistrictId = District?.Id;
            order.StreetId = Street?.Id;
            order.House = House;
            order.Entrance = Entrance;
            order.Floor = Floor;
            order.Intercom = Intercom;
            order.LastName = LastName;
            order.FirstName = separetedNameWithMiddleName[0];
            order.MiddleName = separetedNameWithMiddleName[1];
            order.Phone = Phone;
            order.Miscellaneous = Miscellaneous;
            order.IsOutOfTurn = IsOutOfTurn;
            order.IsProblematicDelivery = IsProblematicDelivery;
            order.Problem = Problem;
            order.CourierId = CourierViewModel?.Id;
            order.CreatedAt = DateTime.UtcNow;

            var ordersService = await Locator.GetInitializedService<IOrdersService>();
            await ordersService.AddOrder(order);

            await loading.CloseAsync();

            await MainViewModel.OkMessage("Заказ сохранен");
            await GoBackCommand.Execute();

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

    public extern string CourierFullName
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public CourierViewModel? CourierViewModel
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> SelectCourierCommand
    {
        get;
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

    public ReactiveCommand<Unit, Unit> SaveCommand
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

        _paymentService = await cashierService.CreatePayment(_orderEdit);
        PaymentPageVm = new(_paymentService);

        await OrderEditPageVm.InitializeAsync();
        await PaymentPageVm.InitializeAsync();

        Disposable.Create(() =>
        {
            _orderEdit.Dispose();
            OrderEditPageVm.Dispose();

            _paymentService.Dispose();
            PaymentPageVm.Dispose();
        }).DisposeWith(disposables);

    }

    private bool IsAllClientInfoFilled()
    {
        return !string.IsNullOrWhiteSpace(NameWithMiddleName) 
            && !string.IsNullOrWhiteSpace(Phone) 
            && !string.IsNullOrWhiteSpace(Address)
            && !string.IsNullOrWhiteSpace(Card);
    }

    private bool IsAllAddressInfoFilled()
    {
        return !string.IsNullOrWhiteSpace(House) 
            && !string.IsNullOrWhiteSpace(Building) 
            && !string.IsNullOrWhiteSpace(Entrance) 
            && !string.IsNullOrWhiteSpace(Floor) 
            && !string.IsNullOrWhiteSpace(Apartment) 
            && !string.IsNullOrWhiteSpace(Intercom)
            && (Street != null)
            && (District != null);
    }
}
