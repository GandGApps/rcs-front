using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.Launcher.Services;
using Kassa.Launcher.Vms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Launcher.Vms;
public sealed class UninstallVm : BaseVm
{

    [Reactive]
    public double Progress
    {
        get; private set;
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    [Reactive]
    public string Status
    {
        get; private set;
    } = "Идет процесс удаление файлов";

    public UninstallVm(IRemover remover)
    {
        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await remover.RemoveAsync(x => Progress = x * 100);

            Status = "Удаление завершено";
        });
    }

}
