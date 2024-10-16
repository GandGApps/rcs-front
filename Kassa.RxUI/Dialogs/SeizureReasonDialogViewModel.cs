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
using Kassa.Shared;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public sealed class SeizureReasonDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<SeizureReasonDto, SeizureReasonVm>
{
    private readonly ISeizureReasonService _seizureReasonService;
    private readonly IAuthService _authService;
    private readonly IFundsService _fundsService;

    public SeizureReasonDialogViewModel(ISeizureReasonService seizureReasonService,IAuthService authService, IFundsService fundsService)
    {
        _seizureReasonService = seizureReasonService;
        _authService = authService;
        _fundsService = fundsService;

        SelectCommand = ReactiveCommand.CreateFromTask<SeizureReasonVm>(async x =>
        {
            SelectedItem = x;

            MemberSelectDialogViewModel memberSelectViewModel = null!;

            memberSelectViewModel = RcsKassa.CreateAndInject<MemberSelectDialogViewModel>(DialogViewModel (MemberDto member) =>
            {
                var fundActDialog = new FundActDialogViewModel
                {
                    ApplyButtonText = "Изъять",
                    HeaderTemplateKey = "SeizureReasonDialog",
                    Reason = x.Name,
                    Member = member.Name,
                    IsRequiredComment = x.SeizureReason.IsRequiredComment
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

                    await MainViewModel.RunTaskWithLoadingDialog("Проводится изъятие", _fundsService.Seize(fundActDialog.Amount, fundActDialog.Comment, member.Id, enterPincodeDialog.Result, x.SeizureReason!));

                    await fundActDialog.CloseAsync();
                    await memberSelectViewModel.CloseAsync();
                    await CloseAsync();

                });
                return fundActDialog;
            });

            memberSelectViewModel.HeaderTemplateKey = "SeizureReasonDialog";

            await MainViewModel.ShowDialogAndWaitClose(memberSelectViewModel);
        });
    }


    protected override void Initialize(CompositeDisposable disposables)
    {
        Filter(_seizureReasonService.RuntimeSeizureReasons, x => new SeizureReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, SeizureReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
