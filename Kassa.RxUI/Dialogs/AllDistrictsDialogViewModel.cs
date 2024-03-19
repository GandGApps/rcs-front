using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class AllDistrictsDialogViewModel : SearchableDialogViewModel<DistrictDto, DistrictViewModel>
{
    private readonly IDistrictService _districtService;

    public AllDistrictsDialogViewModel(IDistrictService districtService)
    {
        _districtService = districtService;

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync);
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get;
        set;
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        Filter(
            _districtService.RuntimeDistricts,
            x => new DistrictViewModel(this, x),
            (vm, source) => vm.Update(source),
            disposables);

        this.WhenAnyValue(x => x.SelectedItem)
            .WhereNotNull()
            .Do(x =>
            {
                CloseCommand.Execute().Subscribe();
            })
            .Subscribe()
            .DisposeWith(disposables);

        return base.InitializeAsync(disposables);
    }

    protected override bool IsMatch(string searchText, DistrictDto item) => item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
}
