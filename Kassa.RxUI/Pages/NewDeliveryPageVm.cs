﻿using System;
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
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public sealed class NewDeliveryPageVm : PageViewModel
{
    private readonly OrderEditDto _orderEdit;
    private IPaymentService _paymentService = null!;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;
    private readonly IProductService _productService;
    private readonly IOrdersService _ordersService;
    private readonly ICategoryService _categoryService;
    private readonly IReceiptService _receiptService;
    private readonly IIngridientsService _ingridientsService;
    private readonly IStorageScope _storageScope;

    public NewDeliveryPageVm(
        ICashierService cashierService,
        IAdditiveService additiveService,
        IProductService productService,
        IOrdersService ordersService,
        ICategoryService categoryService,
        IReceiptService receiptService,
        IIngridientsService ingridientsService,
        OrderEditDto orderEditDto) :
        this(cashierService, additiveService, productService, ordersService, categoryService, receiptService, ingridientsService, orderEditDto, null)
    {
        IsNewClient = true;
    }

    public NewDeliveryPageVm(
        ICashierService cashierService,
        IAdditiveService additiveService,
        IProductService productService,
        IOrdersService ordersService,
        ICategoryService categoryService,
        IReceiptService receiptService,
        IIngridientsService ingridientsService,
        OrderEditDto orderEditDto,
        ClientViewModel? clientViewModel)
    {
        _orderEdit = orderEditDto;

        _cashierService = cashierService;
        _additiveService = additiveService;
        _productService = productService;
        _ordersService = ordersService;
        _categoryService = categoryService;
        _receiptService = receiptService;
        _ingridientsService = ingridientsService;
        _storageScope = _ingridientsService.CreateStorageScope();

        DeliveryId = Guid.NewGuid();
        Client = clientViewModel;

        Phone = Client?.Phone ?? string.Empty;
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
        IsClientOpenned = true;

        this.WhenAnyValue(x => x.IsPickup, x => x.IsDelivery, (isPickup, isDelivery) => isPickup ? "Самовывоз" : isDelivery ? "Доставка курьером" : string.Empty)
            .ToPropertyEx(this, x => x.TypeOfOrder)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.CourierViewModel, (CourierViewModel? x) => x is not null ? x.FullName : string.Empty)
            .ToPropertyEx(this, x => x.CourierFullName)
            .DisposeWith(InternalDisposables);

        SelectDistrictAndStreetCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var districtService = RcsKassa.GetRequiredService<IDistrictService>();

            var districtDialog = new AllDistrictsDialogViewModel(districtService);

            await MainViewModel.ShowDialogAndWaitClose(districtDialog);

            if (districtDialog.SelectedItem is null)
            {
                return;
            }

            var streetService = RcsKassa.GetRequiredService<IStreetService>();

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
            var courierService = RcsKassa.GetRequiredService<ICourierService>();
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
                await MainViewModel.OkMessageAsync("Не выбран курьер");
                return;
            }

            if (IsDelivery && (District is null || Street is null))
            {
                await MainViewModel.OkMessageAsync("Не выбран район или улица");
                return;
            }

            if (!PaymentPageVm.IsExactAmount)
            {
                await MainViewModel.OkMessageAsync("Необходимо ввести сумму оплаты");
                return;
            }

            var order = await _ordersService.CreateOrderAsync(_orderEdit);

            if (!order.Products.Any())
            {
                await MainViewModel.OkMessageAsync("Не выбраны товары");
                return;
            }

            if (!IsAllClientInfoFilled())
            {
                await MainViewModel.OkMessageAsync("Не все данные клиента заполнены");
                return;
            }

            if (!IsAllAddressInfoFilled() && IsDelivery)
            {
                await MainViewModel.OkMessageAsync("Не все данные адреса заполнены");
                return;
            }

            if (IsNewClient)
            {

                if (!IsAllAddressInfoFilled())
                {
                    await MainViewModel.OkMessageAsync("Не все данные адреса заполнены. Они обязательны для нового клиента.");
                    return;
                }

                var client = new ClientDto
                {
                    Id = Guid.NewGuid(),
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
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    Miscellaneous = Miscellaneous,
                    Phone = Phone
                };

                var clientService = RcsKassa.GetRequiredService<IClientService>();
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
            order.FirstName = FirstName;
            order.MiddleName = MiddleName;
            order.Phone = Phone;
            order.Miscellaneous = Miscellaneous;
            order.IsOutOfTurn = IsOutOfTurn;
            order.IsProblematicDelivery = IsProblematicDelivery;
            order.Problem = Problem;
            order.CourierId = CourierViewModel?.Id;
            order.ClientId = Client?.Id;
            order.CreatedAt = DateTime.UtcNow;


            await _paymentService.PayAndSaveOrderThenDispose(ReceiptBehavior.NoPrintReceipt);

            await loading.CloseAsync();

            await MainViewModel.OkMessageAsync("Заказ сохранен");
            await BackButtonCommand.Execute().FirstAsync();

        }).DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsDelivery, x => x.IsNewClient, (isDelivery, isNewClient) => isDelivery || isNewClient)
            .ToPropertyEx(this, x => x.IsAddressInfoVisible)
            .DisposeWith(InternalDisposables);

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

    public ClientViewModel? Client
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

    [Reactive]
    public bool IsNewClient
    {
        get; set;
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

    public extern bool IsAddressInfoVisible
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

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var cashierService = RcsKassa.GetRequiredService<ICashierService>();

        OrderEditPageVm = new DeliveryOrderEditPageVm(_orderEdit, _storageScope, _cashierService, _additiveService, _productService, _categoryService, _receiptService, _ingridientsService);

        _paymentService = await cashierService.CreatePayment(_orderEdit);
        PaymentPageVm = new(OrderEditPageVm.ShoppingList.ProductShoppingListItems, OrderEditPageVm, _paymentService, _cashierService, _additiveService, _productService, _ingridientsService, _receiptService, _categoryService);

        await OrderEditPageVm.InitializeAsync();
        await PaymentPageVm.InitializeAsync();

        // Just being lazy to write DisposeWith for each disposable
        // And being stupid because I also wrote Dispose for each disposable
        // I'll keep this as a reminder
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
