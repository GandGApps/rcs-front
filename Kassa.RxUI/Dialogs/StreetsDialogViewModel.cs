using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class StreetsDialogViewModel : SearchableDialogViewModel<StreetDto, StreetViewModel>
{
    private readonly IStreetService _streets;

    public DistrictViewModel DistrictViewModel
    {
        get;
    }

    public StreetsDialogViewModel(DistrictViewModel districtViewModel, IStreetService streets)
    {
        DistrictViewModel = districtViewModel;

        _streets = streets;
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        Filter(_streets.Streets, street => new StreetViewModel(this, street), (vm, street) => vm.Update(street), disposables);


        this.WhenAnyValue(x => x.SelectedItem)
            .WhereNotNull()
            .Subscribe(x =>
            {

                CloseCommand.Execute().Subscribe();
            })
            .DisposeWith(disposables);

        return ValueTask.CompletedTask;
    }

    protected override bool IsMatch(string searchText, StreetDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) && item.DistrictId == DistrictViewModel.Id;
    }
}
