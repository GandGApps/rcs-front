using System.Reactive.Disposables;
using System.Reactive;
using Kassa.BuisnessLogic.Services;
using Kassa.BuisnessLogic;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive.Linq;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Dialogs;
using Kassa.DataAccess.Model;

namespace Kassa.RxUI.Pages;
public sealed class EditDeliveryPageVm : PageViewModel
{
    private IOrderEditService _orderEdit = null!;
    private IPaymentService _paymentService = null!;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;


    public EditDeliveryPageVm(
        ICashierService cashierService,
        IAdditiveService additiveService,
        ClientDto? client,
        CourierDto? courier,
        OrderDto order,
        DistrictDto? district,
        StreetDto? street)
    {
        _cashierService = cashierService;
        _additiveService = additiveService;

        DeliveryId = order.Id;
        Client = client;

        Phone = order.Phone;
        NameWithMiddleName = $"{order.FirstName} {order.MiddleName}";
        House = order.House;
        Building = order.Building;
        Entrance = order.Entrance;
        Floor = order.Floor;
        Apartment = order.Apartment;
        Intercom = order.Intercom;
        Card = order.Card;
        AddressNote = order.AddressNote;
        LastName = order.LastName;
        FirstName = order.FirstName;
        MiddleName = order.MiddleName;
        Miscellaneous = order.Miscellaneous;
        IsDelivery = order.IsDelivery;
        IsPickup = order.IsPickup;
        IsOutOfTurn = order.IsOutOfTurn;
        Problem = order.Problem;
        IsProblematicDelivery = order.IsProblematicDelivery;
        CourierViewModel = courier is null ? null : new CourierViewModel(courier);
        Street = order.StreetId.HasValue ? new StreetViewModel(street) : null;
        District = order.DistrictId.HasValue ? new DistrictViewModel(district) : null;
        

        IsClientOpenned = true;

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



        BackButtonCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (MainViewModel.Router.GetCurrentViewModel() is not NewDeliveryPageVm)
            {
                await MainViewModel.Router.NavigateBack.Execute().FirstAsync();
            }

            await MainViewModel.GoBackCommand.Execute().FirstAsync();

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

            if (IsDelivery && (District is null || Street is null))
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

            if (!IsAllAddressInfoFilled() && IsDelivery)
            {
                await MainViewModel.OkMessage("Не все данные адреса заполнены");
                return;
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
            order.ClientId = Client?.Id;
            order.CreatedAt = DateTime.UtcNow;

            var ordersService = await Locator.GetInitializedService<IOrdersService>();
            await ordersService.UpdateOrder(order);

            await loading.CloseAsync();

            await MainViewModel.OkMessage("Заказ сохранен");
            await BackButtonCommand.Execute().FirstAsync();

        }).DisposeWith(InternalDisposables);

        EditStatusCommand = ReactiveCommand.Create<OrderStatus>(status =>
        {
            OrderStatus = status;
        }).DisposeWith(InternalDisposables);

#if DEBUG
        Disposable.Create(() =>
        {
        }).DisposeWith(InternalDisposables);
#endif
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

    public ClientDto? Client
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

    public ReactiveCommand<OrderStatus, Unit> EditStatusCommand
    {
        get;
    }

    [Reactive]
    public DeliveryOrderEditPageVm OrderEditPageVm
    {
        get; set;
    } = null!;

    [Reactive]
    public DeliveryPaymentPageVm PaymentPageVm
    {
        get; set;
    } = null!;

    [Reactive]
    public bool IsClientOpenned
    {
        get; set;
    }

    [Reactive]
    public OrderStatus OrderStatus
    {
        get; set;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var cashierService = await Locator.GetInitializedService<ICashierService>();

        _orderEdit = await cashierService.CreateOrder(true);
        await cashierService.SelectCurrentOrder(_orderEdit);

        OrderEditPageVm = new DeliveryOrderEditPageVm(_orderEdit, _cashierService, _additiveService);

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
