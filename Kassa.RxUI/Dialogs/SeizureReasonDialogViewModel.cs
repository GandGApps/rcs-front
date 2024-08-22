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
public sealed class SeizureReasonDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<SeizureReasonDto, SeizureReasonVm>
{
    public SeizureReasonDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<SeizureReasonVm>(async x =>
        {
            SelectedItem = x;

            var memberSelectViewModel = new MemberSelectDialogViewModel(member =>
            {
                var fundActDialog = new FundActDialogViewModel
                {
                    ApplyButtonText = "Изъять",
                    HeaderTemplateKey = "SeizureReasonDialog",
                    Reason = x.Name,
                    Member = member.Name,
                };

                fundActDialog.ApplyCommand.Subscribe(async _ =>
                {
                    var fundsService = await Locator.GetInitializedService<IFundsService>();

                    await fundsService.Seize(fundActDialog.Amount, fundActDialog.Comment, member.Id, "1111", x.SeizureReason!);
                });

                return fundActDialog;
            })
            {
                HeaderTemplateKey = "SeizureReasonDialog"
            };

            await MainViewModel.ShowDialogAndWaitClose(memberSelectViewModel);
        });
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var withdrawalReasons = await Locator.GetInitializedService<IWithdrawReasonService>();

        Filter(withdrawalReasons.RuntimeWithdrawReasouns, x => new SeizureReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, SeizureReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
