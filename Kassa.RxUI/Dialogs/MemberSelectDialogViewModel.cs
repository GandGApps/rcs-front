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
using Kassa.Shared.ServiceLocator;

namespace Kassa.RxUI.Dialogs;
public sealed class MemberSelectDialogViewModel : ApplicationManagedModelSearchableDialogViewModel<MemberDto, MemberVm>
{
    private readonly Func<MemberDto, DialogViewModel?>? _afterSelectAction;

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
    public MemberSelectDialogViewModel(Func<MemberDto, DialogViewModel?>? afterSelectAction = null)
    {
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

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var memberService = RcsLocator.Scoped.GetRequiredService<IMemberService>();

        Filter(memberService.RuntimeMembers, x => new MemberVm(x, this), disposables);
    }

    protected override bool IsMatch(string searchText, MemberDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
