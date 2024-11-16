using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using RcsInstaller.Progress;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RcsInstaller.Vms;
public sealed class ProgressPageVm: PageVm
{
    public ProgressPageVm()
    {
        StartTaskCommand = ReactiveCommand.CreateFromTask<Func<ProgressCalback, Task>>(async task =>
        {
            await task((s, e) =>
            {
                var newProgressState = new ProgressState(e.ProgressState.State, e.Progress * 100);
                ProgressState = newProgressState;
            });
        });
    }

    public async Task StartTaskAndShowProgress(Func<ProgressCalback, Task> task)
    {
        if(HostScreen.Router.GetCurrentViewModel() != this)
        {
            HostScreen.Router.Navigate.Execute(this).Subscribe();
        }

        await StartTaskCommand.Execute(task).FirstAsync();
    }

    /// <summary>
    /// Subscribe to this command to wait for the task to complete
    /// </summary>
    public ReactiveCommand<Func<ProgressCalback, Task>, Unit> StartTaskCommand
    {
        get;
    }

    [Reactive]
    public ProgressState? ProgressState
    {
        get; set;
    }
}
