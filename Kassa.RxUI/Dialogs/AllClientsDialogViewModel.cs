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
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class AllClientsDialogViewModel : SearchableDialogViewModel<ClientDto, ClientViewModel>
{
    private readonly IClientService _clientService;
    private readonly ICashierService _cashierService;

    public AllClientsDialogViewModel(IClientService clientService, ICashierService cashierService)
    {
        _clientService = clientService;
        _cashierService = cashierService;

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync);

        var okCommandValidator = this.WhenAnyValue(x => x.SelectedItem)
                                     .Select(client => client != null);

        SkipCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await CreateOrderAndNavigate();
        });

        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await CreateOrderAndNavigate(SelectedItem!);
        }, okCommandValidator);

        NewGuestCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await CreateOrderAndNavigate(SelectedItem!);
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

    private async Task CreateOrderAndNavigate(object? selectedItem = null)
    {
        var orderEditDto = await _cashierService.CreateOrder(true);

        var newDeliveryPageVm = selectedItem == null
            ? RcsKassa.CreateAndInject<NewDeliveryPageVm>(orderEditDto)
            : RcsKassa.CreateAndInject<NewDeliveryPageVm>(orderEditDto, selectedItem);

        newDeliveryPageVm.IsPickup = IsPickup;
        newDeliveryPageVm.IsDelivery = IsDelivery;

        await CloseAsync();
        await MainViewModel.GoToPageCommand.Execute(newDeliveryPageVm).FirstAsync();
    }
}
