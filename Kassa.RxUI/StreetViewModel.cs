using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class StreetViewModel : ReactiveObject
{
    public StreetViewModel(StreetDto streetDto)
    {
        Id = streetDto.Id;
        Name = streetDto.Name;
        DistrictId = streetDto.DistrictId;

        SelectCommand = ReactiveCommand.Create(() =>
        {

        });
    }

    public StreetViewModel(StreetsDialogViewModel streets, StreetDto streetDto) : this(streetDto)
    {
        SelectCommand = ReactiveCommand.Create(() =>
        {
            streets.SelectedItem = this;
        });
    }

    [Reactive]
    public string Name
    {
        get; set;
    }
    [Reactive]
    public Guid Id
    {
        get; set;
    }

    [Reactive]
    public Guid DistrictId
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> SelectCommand
    {
        get;
    }

    [Reactive]
    public bool IsSelected
    {
        get; set;
    }

    public void Update(StreetDto streetDto)
    {
        Id = streetDto.Id;
        Name = streetDto.Name;
        DistrictId = streetDto.DistrictId;
    }
}
