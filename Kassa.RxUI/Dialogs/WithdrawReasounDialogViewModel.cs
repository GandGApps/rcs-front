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
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public sealed class WithdrawReasounDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<WithdrawalReasonDto, WithdrawalReasonVm>
{
    public WithdrawReasounDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<WithdrawalReasonVm>(async x =>
        {
            SelectedItem = x;

            var memberSelectViewModel = new MemberSelectDialogViewModel();

            await MainViewModel.ShowDialogAndWaitClose(memberSelectViewModel);
        });
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var withdrawalReasons = await Locator.GetInitializedService<IWithdrawReasounService>();

        Filter(withdrawalReasons.RuntimeWithdrawReasouns, x => new WithdrawalReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, WithdrawalReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
