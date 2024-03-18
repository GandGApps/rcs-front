using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;

public class ClientViewModel : ReactiveObject
{
    public ClientViewModel(ClientDto dto, AllClientsDialogViewModel? allClientsDialogView)
    {
        Id = dto.Id;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        MiddleName = dto.MiddleName;
        Address = dto.Address;
        House = dto.House;
        Building = dto.Building;
        Entrance = dto.Entrance;
        Floor = dto.Floor;
        Apartment = dto.Apartment;
        Intercom = dto.Intercom;
        Phone = dto.Phone;
        Card = dto.Card;
        AddressNote = dto.AddressNote;
        Miscellaneous = dto.Miscellaneous;
        AllClientsDialogVm = allClientsDialogView;

        if (allClientsDialogView is null)
        {
            return;
        }

        this.WhenAnyValue(x => x.FirstName, x => x.LastName, x => x.MiddleName, (firstName, lastName, middleName) => $"{firstName} {lastName} {middleName}")
            .ToPropertyEx(this, x => x.FullName);

        allClientsDialogView
            .WhenAnyValue(allClientsDialogView => allClientsDialogView.SearchText)
            .Subscribe(x =>
            {
                IsDetailsVisible = !string.IsNullOrWhiteSpace(x);
            });

        allClientsDialogView
            .WhenAnyValue(allClientsDialogView => allClientsDialogView.SelectedItem)
            .Subscribe(x =>
            {
                IsSelected = x != null && x.Id == Id;
            });

        SelectClientCommand = ReactiveCommand.Create(() =>
        {
            allClientsDialogView.SelectedItem = this;
        });
    }

    public AllClientsDialogViewModel? AllClientsDialogVm
    {
        get;
    }

    [Reactive]
    public Guid Id
    {
        get;
        set;
    }

    [Reactive]
    public string FirstName
    {
        get; set;
    }

    [Reactive]
    public string LastName
    {
        get; set;
    }
    [Reactive]
    public string MiddleName
    {
        get; set;
    }

    public extern string FullName
    {
        [ObservableAsProperty]
        get;
    }


    [Reactive]
    public string Address
    {
        get;
        set;
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
    public string AddressNote
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
    public string Miscellaneous
    {
        get; set;
    }

    [Reactive]
    public bool IsSelected
    {
        get;
        set;
    }

    [Reactive]
    public bool IsDetailsVisible
    {
        get;
        set;
    }

    public ReactiveCommand<Unit, Unit>? SelectClientCommand
    {
        get;
    }

    public void UpdateDto(ClientDto dto)
    {
        Id = dto.Id;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        MiddleName = dto.MiddleName;
        Address = dto.Address;
        House = dto.House;
        Building = dto.Building;
        Entrance = dto.Entrance;
        Floor = dto.Floor;
        Apartment = dto.Apartment;
        Intercom = dto.Intercom;
        Phone = dto.Phone;
        Card = dto.Card;
        AddressNote = dto.AddressNote;
        Miscellaneous = dto.Miscellaneous;
    }
}
