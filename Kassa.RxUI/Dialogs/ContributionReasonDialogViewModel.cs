using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Reactive;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using Kassa.Shared;

namespace Kassa.RxUI.Dialogs;
public sealed class ContributionReasonDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<ContributionReasonDto, ContributionReasonVm>
{
    public ContributionReasonDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<ContributionReasonVm>(async x =>
        {
            SelectedItem = x;

            MemberSelectDialogViewModel memberSelectViewModel = null!;

            memberSelectViewModel = new MemberSelectDialogViewModel(member =>
            {
                var fundActDialog = new FundActDialogViewModel
                {
                    ApplyButtonText = "Внести",
                    HeaderTemplateKey = "ContributionReasonDialog",
                    Reason = x.Name,
                    Member = member.Name,
                };

                fundActDialog.ApplyCommand = ReactiveCommand.CreateFromTask(async _ =>
                {
                    var enterPincodeDialog = new EnterPincodeDialogViewModel();

                    await MainViewModel.ShowDialogAndWaitClose(enterPincodeDialog);

                    var authService = Locator.GetRequiredService<IAuthService>();

                    if (string.IsNullOrWhiteSpace(enterPincodeDialog.Result))
                    {
                        return;
                    }

                    var check = await MainViewModel.RunTaskWithLoadingDialog("Проверка пинкода", authService.CheckPincode(member, enterPincodeDialog.Result));

                    if (!check)
                    {
                        await MainViewModel.OkMessage("Неверный пинкод");

                        await fundActDialog.CloseAsync();
                        await memberSelectViewModel.CloseAsync();
                        await CloseAsync();

                        return;
                    }

                    var fundsService = await Locator.GetInitializedService<IFundsService>();

                    await MainViewModel.RunTaskWithLoadingDialog("Проводиться внесение", fundsService.Contribute(fundActDialog.Amount, fundActDialog.Comment, member.Id, enterPincodeDialog.Result, x.ContributionReason!));

                    await fundActDialog.CloseAsync();
                    await memberSelectViewModel.CloseAsync();
                    await CloseAsync();

                });
                return fundActDialog;
            })
            {
                HeaderTemplateKey = "ContributionReasonDialog"
            };

            await MainViewModel.ShowDialogAndWaitClose(memberSelectViewModel);
        });
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var withdrawalReasons = await Locator.GetInitializedService<IContributionReasonService>();

        Filter(withdrawalReasons.RuntimeContributionReasons, x => new ContributionReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, ContributionReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
