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
    private OrderEditDto _orderEdit = null!;
    private IPaymentService _paymentService = null!;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;
    private readonly IProductService _productService;
    private readonly IReceiptService _receiptService;
    private readonly IIngridientsService _ingridientsService;
    private readonly ICategoryService _categoryService;
    private readonly OrderDto _orderDto;

    public EditDeliveryPageVm(
        ICashierService cashierService,
        IAdditiveService additiveService,
        ClientDto? client,
        CourierDto? courier,
        OrderDto order,
        DistrictDto? district,
        StreetDto? street,
        IProductService productService,
        IReceiptService receiptService,
        IIngridientsService ingridientsService,
        ICategoryService categoryService)
    {
        _orderDto = order;
        _cashierService = cashierService;
        _additiveService = additiveService;
        _productService = productService;
        _receiptService = receiptService;
        _ingridientsService = ingridientsService;
        _categoryService = categoryService;

        DeliveryId = order.Id;
        Client = client;

        Phone = order.Phone;
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
        Street = order.StreetId.HasValue ? new StreetViewModel(street!) : null;
        District = order.DistrictId.HasValue ? new DistrictViewModel(district!) : null;


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
            if (MainViewModel.Router.GetCurrentViewModel() != this)
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

            var orderEdit = _orderEdit;

            if (orderEdit.Products.Count == 0)
            {
                await MainViewModel.OkMessage("Не выбраны товары");
                return;
            }

            if (!IsAllClientInfoFilled())
            {
                await MainViewModel.OkMessage("Не все данные клиента заполнены");
                return;
            }

            if (!IsAllAddressInfoFilled() && IsDelivery)
            {
                await MainViewModel.OkMessage("Не все данные адреса заполнены");
                return;
            }

            var loading = MainViewModel.ShowLoadingDialog("Сохранение заказа");

            orderEdit.AddressNote = AddressNote;
            orderEdit.Comment = Comment;
            orderEdit.Apartment = Apartment;
            orderEdit.Building = Building;
            orderEdit.Card = Card;
            orderEdit.Id = DeliveryId;
            orderEdit.IsDelivery = IsDelivery;
            orderEdit.IsPickup = IsPickup;
            orderEdit.DistrictId = District?.Id;
            orderEdit.StreetId = Street?.Id;
            orderEdit.House = House;
            orderEdit.Entrance = Entrance;
            orderEdit.Floor = Floor;
            orderEdit.Intercom = Intercom;
            orderEdit.LastName = LastName;
            orderEdit.FirstName = FirstName;
            orderEdit.MiddleName = MiddleName;
            orderEdit.Phone = Phone;
            orderEdit.Miscellaneous = Miscellaneous;
            orderEdit.IsOutOfTurn = IsOutOfTurn;
            orderEdit.IsProblematicDelivery = IsProblematicDelivery;
            orderEdit.Problem = Problem;
            orderEdit.CourierId = CourierViewModel?.Id;
            orderEdit.ClientId = Client?.Id;
            orderEdit.CreatedAt = DateTime.UtcNow;
            orderEdit.Status = OrderStatus;

            var ordersService = await Locator.GetInitializedService<IOrdersService>();

            var order = await ordersService.CreateOrderAsync(orderEdit);

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
    [Obsolete("Use name and middlename instead")]
    public string NameWithMiddleName
    {
        get; set;
    } = string.Empty;

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

        _orderEdit = await cashierService.CreateOrder(_orderDto);

        var storageScope = _ingridientsService.CreateStorageScope();

        OrderEditPageVm = new DeliveryOrderEditPageVm(_orderEdit, storageScope, _cashierService, _additiveService, _productService, _categoryService, _receiptService, _ingridientsService);

        _paymentService = await cashierService.CreatePayment(_orderEdit);
        PaymentPageVm = new(OrderEditPageVm.ShoppingList.ProductShoppingListItems, OrderEditPageVm, _paymentService, _cashierService, _additiveService, _productService, _ingridientsService, _receiptService, _categoryService);

        await OrderEditPageVm.InitializeAsync();
        await PaymentPageVm.InitializeAsync();

        Disposable.Create(() =>
        {
            OrderEditPageVm.Dispose();

            _paymentService.Dispose();
            PaymentPageVm.Dispose();
        }).DisposeWith(disposables);

    }

    private bool IsAllClientInfoFilled()
    {
        return !string.IsNullOrWhiteSpace(FirstName)
            && !string.IsNullOrWhiteSpace(LastName)
            && !string.IsNullOrWhiteSpace(MiddleName)
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
