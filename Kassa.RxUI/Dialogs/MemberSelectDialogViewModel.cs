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
    private readonly Func<MemberDto, DialogViewModel?>? _afterSelectAction;
    private readonly IMemberService _memberService;

    [Reactive]
    public object? HeaderTemplateKey
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="afterSelectAction">When a member is selected, you can return a new dialog instance, which will be displayed, or return null to close the dialog.</param>
    public MemberSelectDialogViewModel(IMemberService memberService, Func<MemberDto, DialogViewModel?>? afterSelectAction = null)
    {
        _memberService = memberService;
        _afterSelectAction = afterSelectAction;

        SelectedMemberCommand = ReactiveCommand.Create<MemberDto, MemberDto>(x => x);

        SelectCommand = ReactiveCommand.CreateFromTask<MemberVm>(async x =>
        {
            x.IsSelected = true;
            SelectedItem = x;

            await SelectedMemberCommand.Execute(x.MemberDto);

            if(_afterSelectAction == null)
            {
                return;
            }

            var dialog = _afterSelectAction(x.MemberDto);

            if(dialog != null)
            {
                await MainViewModel.ShowDialogAndWaitClose(dialog);
            }
            else
            {
                await CloseCommand.Execute();
            }
        });
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        Filter(_memberService.RuntimeMembers, x => new MemberVm(x, this), disposables);

        return ValueTask.CompletedTask;
    }

    protected override bool IsMatch(string searchText, MemberDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
