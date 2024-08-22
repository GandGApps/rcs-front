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
public sealed class ContributionReasonVm: ReactiveObject, IApplicationModelPresenter<ContributionReasonDto>
{
    private readonly ContributionReasonDialogViewModel? _dialogViewModel;

    public Guid Id
    {
        get;
    }
    
    public ContributionReasonDto? ContributionReason
    {
        get; private set;
    }

    public ContributionReasonVm(ContributionReasonDto ContributionReason, ContributionReasonDialogViewModel dialogViewModel) : this(ContributionReason)
    {
        _dialogViewModel = dialogViewModel;

    }

    public ContributionReasonVm(ContributionReasonDto contributionReason)
    {
        Id = contributionReason.Id;
        Name = contributionReason.Name;
        ContributionReason = contributionReason;

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

    public void ModelChanged(Change<ContributionReasonDto> change)
    {
        var model = change.Current;

        ContributionReason = model;
        Name = model.Name;
    }

    public void Dispose()
    {

    }
}
