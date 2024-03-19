using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class DistrictViewModel : ReactiveObject
{

    public DistrictViewModel(AllDistrictsDialogViewModel districtsDialogViewModel, DistrictDto districtDto)
    {
        Id = districtDto.Id;
        Name = districtDto.Name;

        SelectCommand = ReactiveCommand.Create(() =>
        {
            districtsDialogViewModel.SelectedItem = this;
        });

        districtsDialogViewModel
            .WhenAnyValue(x => x.SelectedItem)
            .Subscribe(selectedItem =>
            {
                IsSelected = selectedItem == this;
            });
    }

    public ReactiveCommand<Unit, Unit> SelectCommand
    {
        get;
        set;
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
