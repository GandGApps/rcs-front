using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class AllClientsDialogViewModel : SearchableDialogViewModel<ClientDto, ClientViewModel>
{
    private readonly IClientService _clientService;
    

    public AllClientsDialogViewModel(IClientService clientService)
    {
        _clientService = clientService;

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync);

        var okCommandValidator = this.WhenAnyValue(x => x.SelectedItem)
                                     .Select(client => client != null);

        SkipCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var cashierService = RcsLocator.Scoped.GetRequiredService<ICashierService>();
            var additiveService = RcsLocator.Scoped.GetRequiredService<IAdditiveService>();
            var productService = RcsLocator.Scoped.GetRequiredService<IProductService>();
            var ordersService = RcsLocator.Scoped.GetRequiredService<IOrdersService>();
            var categoryService = RcsLocator.Scoped.GetRequiredService<ICategoryService>();
            var receiptService = RcsLocator.Scoped.GetRequiredService<IReceiptService>();
            var ingridientsService = RcsLocator.Scoped.GetRequiredService<IIngridientsService>();

            var orderEditDto = await cashierService.CreateOrder(true);

            var newDeliveryPageVm = new NewDeliveryPageVm(orderEditDto, cashierService, additiveService, productService, ordersService, categoryService, receiptService, ingridientsService)
            {
                IsPickup = IsPickup,
                IsDelivery = IsDelivery
            };

            await CloseAsync();

            await MainViewModel.GoToPageCommand.Execute(newDeliveryPageVm).FirstAsync();
        });

        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var cashierService = RcsLocator.Scoped.GetRequiredService<ICashierService>();
            var additiveService = RcsLocator.Scoped.GetRequiredService<IAdditiveService>();
            var productService = RcsLocator.Scoped.GetRequiredService<IProductService>();
            var ordersService = RcsLocator.Scoped.GetRequiredService<IOrdersService>();
            var categoryService = RcsLocator.Scoped.GetRequiredService<ICategoryService>();
            var receiptService = RcsLocator.Scoped.GetRequiredService<IReceiptService>();
            var ingridientsService = RcsLocator.Scoped.GetRequiredService<IIngridientsService>();

            var orderEditDto = await cashierService.CreateOrder(true);

            var newDeliveryPageVm = new NewDeliveryPageVm(orderEditDto, cashierService, additiveService, SelectedItem, productService, ordersService, categoryService, receiptService, ingridientsService)
            {
                IsPickup = IsPickup,
                IsDelivery = IsDelivery
            };

            await CloseAsync();

            await MainViewModel.GoToPageCommand.Execute(newDeliveryPageVm).FirstAsync();

        }, okCommandValidator);

        NewGuestCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var cashierService = RcsLocator.Scoped.GetRequiredService<ICashierService>();
            var additiveService = RcsLocator.Scoped.GetRequiredService<IAdditiveService>();
            var productService = RcsLocator.Scoped.GetRequiredService<IProductService>();
            var ordersService = RcsLocator.Scoped.GetRequiredService<IOrdersService>();
            var categoryService = RcsLocator.Scoped.GetRequiredService<ICategoryService>();
            var receiptService = RcsLocator.Scoped.GetRequiredService<IReceiptService>();
            var ingridientsService = RcsLocator.Scoped.GetRequiredService<IIngridientsService>();

            var orderEditDto = await cashierService.CreateOrder(true);

            var newDeliveryPageVm = new NewDeliveryPageVm(orderEditDto, cashierService, additiveService, SelectedItem, productService, ordersService, categoryService, receiptService, ingridientsService)
            {
                IsPickup = IsPickup,
                IsDelivery = IsDelivery
            };

            await CloseAsync();

            await MainViewModel.GoToPageCommand.Execute(newDeliveryPageVm).FirstAsync();
        });

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

    public ReactiveCommand<Unit, Unit> NewGuestCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SkipCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OkCommand
    {
        get;
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        Filter(
            _clientService.RuntimeClients,
            client => new ClientViewModel(client, this),
            (vm, source) => vm.UpdateDto(source),
            disposables);

        return base.InitializeAsync(disposables);
    }

    protected override bool IsMatch(string text, ClientDto client)
    {
        return client.Address.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.FirstName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.LastName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.MiddleName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.Phone.Contains(text, StringComparison.OrdinalIgnoreCase);
    }
}
