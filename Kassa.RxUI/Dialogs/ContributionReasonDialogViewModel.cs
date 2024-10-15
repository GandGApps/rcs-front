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
using Kassa.Shared.ServiceLocator;

namespace Kassa.RxUI.Dialogs;
public sealed class ContributionReasonDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<ContributionReasonDto, ContributionReasonVm>
{
    private readonly IAuthService _authService;
    private readonly IFundsService _fundsService;
    private readonly IContributionReasonService _contributionReasonService;

    public ContributionReasonDialogViewModel(IAuthService authService, IFundsService fundsService, IContributionReasonService contributionReasonService)
    {
        _authService = authService;
        _fundsService = fundsService;
        _contributionReasonService = contributionReasonService;

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
                    IsRequiredComment = x.ContributionReason.IsRequiredComment
                };

                fundActDialog.ApplyCommand = ReactiveCommand.CreateFromTask(async _ =>
                {
                    var enterPincodeDialog = new EnterPincodeDialogViewModel();

                    await MainViewModel.ShowDialogAndWaitClose(enterPincodeDialog);

                    if (string.IsNullOrWhiteSpace(enterPincodeDialog.Result))
                    {
                        return;
                    }

                    var check = await MainViewModel.RunTaskWithLoadingDialog("Проверка пинкода", _authService.CheckPincode(member, enterPincodeDialog.Result));

                    if (!check)
                    {
                        await MainViewModel.OkMessageAsync("Неверный пинкод");

                        await fundActDialog.CloseAsync();
                        await memberSelectViewModel.CloseAsync();
                        await CloseAsync();

                        return;
                    }

                    await MainViewModel.RunTaskWithLoadingDialog("Проводиться внесение", _fundsService.Contribute(fundActDialog.Amount, fundActDialog.Comment, member.Id, enterPincodeDialog.Result, x.ContributionReason!));

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


    protected override void Initialize(CompositeDisposable disposables)
    {
        Filter(_contributionReasonService.RuntimeContributionReasons, x => new ContributionReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, ContributionReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
