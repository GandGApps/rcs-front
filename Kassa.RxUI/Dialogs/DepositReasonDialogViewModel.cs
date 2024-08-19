using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public sealed class DepositReasonDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<DepositReasonDto, DepositReasonVm>
{
    public DepositReasonDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<DepositReasonVm>(async x =>
        {
            SelectedItem = x;

            var memberSelectViewModel = new MemberSelectDialogViewModel(member =>
            {
                var fundActDialog = new FundActDialogViewModel
                {
                    ApplyButtonText = "Внести",
                    HeaderTemplateKey = "DepositReasonDialog",
                    Reason = x.Name,
                    Member = member.Name,
                };

                return fundActDialog;
            })
            {
                HeaderTemplateKey = "DepositReasonDialog"
            };

            await MainViewModel.ShowDialogAndWaitClose(memberSelectViewModel);
        });
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var withdrawalReasons = await Locator.GetInitializedService<IDepositReasonService>();

        Filter(withdrawalReasons.RuntimeDepositReasons, x => new DepositReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, DepositReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
