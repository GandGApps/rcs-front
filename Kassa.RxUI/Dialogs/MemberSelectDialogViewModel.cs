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
using System.Reactive;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace Kassa.RxUI.Dialogs;
public sealed class MemberSelectDialogViewModel : ApplicationManagedModelSearchableDialogViewModel<MemberDto, MemberVm>
{

    [Reactive]
    public object? Header
    {
        get; set;
    }

    [Reactive]
    public string? Icon
    {
        get; set;
    }

    public ReactiveCommand<MemberDto, MemberDto> SelectedMemberCommand
    {
        get;
    }

    public MemberSelectDialogViewModel()
    {

        SelectedMemberCommand = ReactiveCommand.Create<MemberDto, MemberDto>(x => x);

        SelectCommand = ReactiveCommand.CreateFromTask<MemberVm>(async x =>
        {
            SelectedItem = x;

            await SelectedMemberCommand.Execute(x.MemberDto);
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
