using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Vms;

public sealed class SelectPathVm : PageVm
{

    public SelectPathVm()
    {
        InstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        InstallCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var vm = new InstallingVm(InstallPath, IsShortcutNeeded, null);
            await HostScreen.Router.Navigate.Execute(vm).FirstAsync();

            await vm.StartInstallCommand.Execute().FirstAsync();
        });
    }

    [Reactive]
    public string InstallPath
    {
        get; set;
    }

    [Reactive]
    public bool IsShortcutNeeded
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> InstallCommand
    {
        get;
    }

}
