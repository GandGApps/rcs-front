using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public abstract class DialogViewModel : BaseViewModel, IRoutableViewModel
{
    private readonly TaskCompletionSource _taskCompletionSource;
    private readonly Subject<bool> _onClose = new();

    public DialogViewModel() : base()
    {
        HostScreen = null!;
        _taskCompletionSource = new();
    
        CloseCommand = ReactiveCommand.Create(SetCloseResult, _onClose).DisposeWith(InternalDisposables);
    
        _onClose.OnNext(true);
        _onClose.DisposeWith(InternalDisposables);
    }

    public virtual ReactiveCommand<Unit,Unit> CloseCommand
    {
        get;
    }
    public string? UrlPathSegment
    {
        get;
    }
    public IScreen HostScreen
    {
        get;
    }

    /// <summary>
    /// Do not call this method directly. Use <see cref="CloseAsync"/> instead.
    /// Or use <see cref="CloseCommand"/> if you want to close dialog from view.
    /// </summary>
    protected virtual void SetCloseResult()
    {
        _taskCompletionSource.SetResult();
        _onClose.OnNext(false);
        _onClose.Dispose();
    }

    public async Task CloseAsync() => await CloseCommand.Execute().FirstAsync();

    public Task WaitDialogClose()
    {
        return _taskCompletionSource.Task;
    }
}
