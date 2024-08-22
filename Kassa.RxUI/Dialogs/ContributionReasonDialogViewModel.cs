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
public sealed class ContributionReasonDialogViewModel: ApplicationManagedModelSearchableDialogViewModel<ContributionReasonDto, ContributionReasonVm>
{
    public ContributionReasonDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<ContributionReasonVm>(async x =>
        {
            SelectedItem = x;

            var memberSelectViewModel = new MemberSelectDialogViewModel(member =>
            {
                var fundActDialog = new FundActDialogViewModel
                {
                    ApplyButtonText = "Внести",
                    HeaderTemplateKey = "ContributionReasonDialog",
                    Reason = x.Name,
                    Member = member.Name,
                };

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
