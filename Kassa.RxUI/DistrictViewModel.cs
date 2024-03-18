using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class DistrictViewModel : ReactiveObject
{

    public DistrictViewModel(DistrictDto districtDto)
    {
        Id = districtDto.Id;
        Name = districtDto.Name;
    }

    [Reactive]
    public Guid Id
    {
        get; set;
    }

    [Reactive]
    public string Name
    {
        get; set;
    } = string.Empty;

    [Reactive]
    public bool IsSelected
    {
        get; set;
    }

    public void Update(DistrictDto districtDto)
    {
        Id = districtDto.Id;
        Name = districtDto.Name;
    }
}
