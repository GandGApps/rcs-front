using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
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

    public void Update(StreetDto streetDto)
    {
        Id = streetDto.Id;
        Name = streetDto.Name;
        DistrictId = streetDto.DistrictId;
    }
}
