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
public sealed class SeizureReasonVm : ReactiveObject, IApplicationModelPresenter<SeizureReasonDto>
{
    private readonly SeizureReasonDialogViewModel? _dialogViewModel;

    public Guid Id
    {
        get;
    }

    public SeizureReasonVm(SeizureReasonDto withdrawalReason, SeizureReasonDialogViewModel dialogViewModel) : this(withdrawalReason)
    {
        _dialogViewModel = dialogViewModel;

    }

    public SeizureReasonVm(SeizureReasonDto withdrawalReason)
    {
        Id = withdrawalReason.Id;
        Name = withdrawalReason.Name;

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

    public void ModelChanged(Change<SeizureReasonDto> change)
    {
        var model = change.Current;

        Name = model.Name;
    }

    public void Dispose()
    {

    }
}
