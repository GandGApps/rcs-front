using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class NewDeliveryPageVm : PageViewModel
{

    public NewDeliveryPageVm() : this(null)
    {
        IsNewClient = true;
    }

    public NewDeliveryPageVm(ClientViewModel? clientViewModel)
    {
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

            District = districtDialog.SelectedItem;

            var streetService = await Locator.GetInitializedService<IStreetService>();
        });
    }

    public Guid DeliveryId
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

    public ReactiveCommand<Unit, Unit> SelectDistrictAndStreetCommand
    {
        get;
    }
}
