using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public sealed class DepositReasonVm: ReactiveObject, IApplicationModelPresenter<DepositReasonDto>
{
    private readonly DepositReasonDialogViewModel? _dialogViewModel;

    public Guid Id
    {
        get;
    }

    public DepositReasonVm(DepositReasonDto depositReason, DepositReasonDialogViewModel dialogViewModel) : this(depositReason)
    {
        _dialogViewModel = dialogViewModel;

    }

    public DepositReasonVm(DepositReasonDto depositReason)
    {
        Id = depositReason.Id;
        Name = depositReason.Name;

        SelectCommand = ReactiveCommand.Create(() =>
        {
            _dialogViewModel?.SelectCommand?.Execute(this);
        });
    }

    [Reactive]
    public string Name
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> SelectCommand
    {
        get;
    }

    public void ModelChanged(Change<DepositReasonDto> change)
    {
        var model = change.Current;

        Name = model.Name;
    }

    public void Dispose()
    {

    }
}
