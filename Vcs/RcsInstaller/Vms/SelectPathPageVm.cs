using RcsInstaller.Progress;
using RcsInstaller.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TruePath;

namespace RcsInstaller.Vms;

public sealed class SelectPathPageVm : PageVm
{
    private readonly IInstaller _installer;
    private readonly IUpdater _updater;

    public SelectPathPageVm(IInstaller installer, IUpdater updater)
    {
        _installer = installer;
        _updater = updater;

        InstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        InstallCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AbsolutePath absoluteInstallPath = new(InstallPath);

            async Task InstallTask(ProgressCalback progress) => await _installer.InstallAsync(absoluteInstallPath, IsShortcutNeeded, state => progress(this, new(state)));

            var progressPageVm = new ProgressPageVm();

            HostScreen.Router.Navigate.Execute(progressPageVm).Subscribe();

            await progressPageVm.StartTaskCommand.Execute(InstallTask);

            await HostScreen.Router.Navigate.Execute(App.CreateInstance<CompletePageVm>(absoluteInstallPath, HelperExtensions.EmptyVersion)).FirstAsync();
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
