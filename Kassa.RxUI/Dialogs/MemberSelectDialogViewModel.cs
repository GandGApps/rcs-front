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
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public sealed class MemberSelectDialogViewModel : ApplicationManagedModelSearchableDialogViewModel<MemberDto, MemberVm>
{

    [Reactive]
    public string? Header
    {
        get; set;
    }

    [Reactive]
    public string? Icon
    {
        get; set;
    }

    public MemberSelectDialogViewModel()
    {
        SelectCommand = ReactiveCommand.CreateFromTask<MemberVm>(async x =>
        {
            SelectedItem = x;
        });
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var memberService = await Locator.GetInitializedService<IMemberService>();

        Filter(memberService.RuntimeMembers, x => new MemberVm(x), disposables);
    }

    protected override bool IsMatch(string searchText, MemberDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
