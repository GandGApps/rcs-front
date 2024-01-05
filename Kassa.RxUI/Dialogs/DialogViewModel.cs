using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public abstract class DialogViewModel : BaseViewModel
{
    private readonly TaskCompletionSource _taskCompletionSource;
    private readonly Subject<bool> _onClose = new();
    public DialogViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        _taskCompletionSource = new();

        CloseCommand = ReactiveCommand.Create(Close, _onClose);

        _onClose.OnNext(true);
    }

    public virtual ICommand CloseCommand
    {
        get;
    }

    public virtual void Close()
    {
        _taskCompletionSource.SetResult();
        _onClose.OnNext(false);
        _onClose.Dispose();
    }

    public Task ShowDialogAsync()
    {
        MainViewModel.DialogOpenCommand.Execute(this).Subscribe();

        return _taskCompletionSource.Task;
    }

    public Task WaitDialogClose()
    {
        return _taskCompletionSource.Task;
    }
}
