using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class SearchCourierDialogViewModel : ApplicationManagedModelSearchableDialogViewModel<CourierDto, CourierViewModel>
{
    private readonly ICourierService _courierService;


    public SearchCourierDialogViewModel(ICourierService courierService)
    {
        _courierService = courierService;

        var okCommandCanExecute = this.WhenAnyValue(x => x.SelectedItem)
            .Select(x => x != null);

        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await CloseAsync();
            return SelectedItem!;
        }, okCommandCanExecute).DisposeWith(InternalDisposables);

        CancelCommand = ReactiveCommand.Create(Close).DisposeWith(InternalDisposables);
    }

    public ReactiveCommand<Unit, CourierViewModel> OkCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get;
    }

    protected override bool IsMatch(string searchText, CourierDto item)
    {
        return item.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
               item.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
               item.MiddleName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
               item.Phone.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        Filter(_courierService.RuntimeCouriers, x => new CourierViewModel(x, this), disposables);

        OkCommand.DisposeWith(disposables);

        return base.InitializeAsync(disposables);
    }
}