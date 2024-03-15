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
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class AllClientsDialogViewModel : DialogViewModel
{
    private readonly IClientService _clientService;

    public AllClientsDialogViewModel(IClientService clientService)
    {
        _clientService = clientService;

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync);

        var okCommandValidator = this.WhenAnyValue(x => x.SelectedClient)
                                     .Select(client => client != null);

        SkipCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var newDeliveryPageVm = new NewDeliveryPageVm(null)
            {
                IsPickup = IsPickup,
                IsDelivery = IsDelivery
            };

            await CloseAsync();

            await MainViewModel.GoToPageCommand.Execute(newDeliveryPageVm).FirstAsync();
        });

        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var newDeliveryPageVm = new NewDeliveryPageVm(SelectedClient)
            {
                IsPickup = IsPickup,
                IsDelivery = IsDelivery
            };

            await CloseAsync();

            await MainViewModel.GoToPageCommand.Execute(newDeliveryPageVm).FirstAsync();

        }, okCommandValidator);

    }

    [Reactive]
    public string? SearchedText
    {
        get;
        set;
    }

    [Reactive]
    public ClientViewModel? SelectedClient
    {
        get; set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ClientViewModel>? FilteredClients
    {
        get; private set;
    }

    [Reactive]
    public bool IsKeyboardVisible
    {
        get;
        set;
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
        var searchTextStream = this.WhenAnyValue(x => x.SearchedText).Publish();

        var firstItemStream = searchTextStream
            .Take(1)
            .ObserveOn(RxApp.MainThreadScheduler);

        var throttledStream = searchTextStream
            .Skip(1)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Merge(firstItemStream)
            .DistinctUntilChanged();

        var searchFilter = throttledStream
            .Select(text => new Func<ClientDto, bool>(client =>
                           string.IsNullOrWhiteSpace(text) || IsMatch(client, text)));

        _clientService.RuntimeClients
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(searchFilter)
            .TransformWithInlineUpdate(client => new ClientViewModel(client, this), (vm, source) => vm.UpdateDto(source))
            .Bind(out var _filteredClients)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposables);

        searchTextStream.Connect().DisposeWith(disposables);

        FilteredClients = _filteredClients;

        return base.InitializeAsync(disposables);
    }

    private static bool IsMatch(ClientDto client, string text)
    {
        return client.Address.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.FirstName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.LastName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.MiddleName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
               client.Phone.Contains(text, StringComparison.OrdinalIgnoreCase);
    }
}
