using System;
using System.Collections.Generic;
using System.Linq;
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
    public ClientViewModel(ClientDto dto, AllClientsDialogViewModel allClientsDialogView)
    {
        Id = dto.Id;
        FullName = dto.FullName;
        Address = dto.Address;
        Phone = dto.Phone;
        AllClientsDialogVm = allClientsDialogView;

        allClientsDialogView
            .WhenAnyValue(allClientsDialogView => allClientsDialogView.SearchedText)
            .Subscribe(x =>
            {
                IsDetailsVisible = !string.IsNullOrWhiteSpace(x);
            });

        allClientsDialogView
            .WhenAnyValue(allClientsDialogView => allClientsDialogView.SelectedClient)
            .Subscribe(x =>
            {
                IsSelected = x != null && x.Id == Id;
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
    public string FullName
    {
        get;
        set;
    }

    [Reactive]
    public string Address
    {
        get;
        set;
    }

    [Reactive]
    public string Phone
    {
        get;
        set;
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


    public void UpdateDto(ClientDto dto)
    {
        Id = dto.Id;
        FullName = dto.FullName;
        Address = dto.Address;
        Phone = dto.Phone;
    }
}
