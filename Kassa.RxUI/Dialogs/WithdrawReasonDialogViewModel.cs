using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public sealed class WithdrawReasonDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<WithdrawalReasonDto, WithdrawalReasonVm>
{
    public WithdrawReasonDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<WithdrawalReasonVm>(async x =>
        {
            SelectedItem = x;

            var memberSelectViewModel = new MemberSelectDialogViewModel(member =>
            {
                var fundActDialog = new FundActDialogViewModel
                {
                    ApplyButtonText = "Изъять",
                    HeaderTemplateKey = "WithdrawReasonDialog",
                    Reason = x.Name,
                    Member = member.Name,
                };

                return fundActDialog;
            })
            {
                HeaderTemplateKey = "WithdrawReasonDialog"
            };

            await MainViewModel.ShowDialogAndWaitClose(memberSelectViewModel);
        });
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var withdrawalReasons = RcsLocator.Scoped.GetRequiredService<IWithdrawReasonService>();

        Filter(withdrawalReasons.RuntimeWithdrawReasouns, x => new WithdrawalReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, WithdrawalReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
