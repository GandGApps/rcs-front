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
    public SeizureReasonDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<SeizureReasonVm>(async x =>
        {
            SelectedItem = x;

            MemberSelectDialogViewModel memberSelectViewModel = null!;

            memberSelectViewModel = new MemberSelectDialogViewModel(member =>
            {
                var fundActDialog = new FundActDialogViewModel
                {
                    ApplyButtonText = "Изъять",
                    HeaderTemplateKey = "SeizureReasonDialog",
                    Reason = x.Name,
                    Member = member.Name,
                };

                fundActDialog.ApplyCommand = ReactiveCommand.CreateFromTask(async _ =>
                {
                    var enterPincodeDialog = new EnterPincodeDialogViewModel();

                    await MainViewModel.ShowDialogAndWaitClose(enterPincodeDialog);

                    var authService = RcsLocator.GetRequiredService<IAuthService>();

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

                    var fundsService = RcsLocator.Scoped.GetRequiredService<IFundsService>();

                    await MainViewModel.RunTaskWithLoadingDialog("Проводится изъятие", fundsService.Seize(fundActDialog.Amount, fundActDialog.Comment, member.Id, enterPincodeDialog.Result, x.SeizureReason!));

                    await fundActDialog.CloseAsync();
                    await memberSelectViewModel.CloseAsync();
                    await CloseAsync();

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
        var withdrawalReasons = RcsLocator.Scoped.GetRequiredService<ISeizureReasonService>();

        Filter(withdrawalReasons.RuntimeSeizureReasons, x => new SeizureReasonVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, SeizureReasonDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
