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

public sealed class SelectPathPageVm : PageVm
{

    public SelectPathPageVm()
    {
        InstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        InstallCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var vm = App.CreateInstance<InstallingPageVm>(InstallPath, IsShortcutNeeded, HelperExtensions.EmptyVersion);
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
