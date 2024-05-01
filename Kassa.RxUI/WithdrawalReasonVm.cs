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
public sealed class WithdrawalReasonVm : ReactiveObject, IApplicationModelPresenter<WithdrawalReasonDto>
{
    private readonly WithdrawReasounDialogViewModel? _dialogViewModel;

    public Guid Id
    {
        get;
    }

    public WithdrawalReasonVm(WithdrawalReasonDto withdrawalReason, WithdrawReasounDialogViewModel dialogViewModel) : this(withdrawalReason)
    {
        _dialogViewModel = dialogViewModel;

    }

    public WithdrawalReasonVm(WithdrawalReasonDto withdrawalReason)
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

    public void ModelChanged(Change<WithdrawalReasonDto> change)
    {
        var model = change.Current;

        Name = model.Name;
    }

    public void Dispose()
    {

    }
}
