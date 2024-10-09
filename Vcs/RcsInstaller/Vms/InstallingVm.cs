using Avalonia.Threading;
using RcsInstaller.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Vms;
public sealed class InstallingVm : PageVm
{
    private readonly string _path;
    private readonly bool _createShortcut;
    private readonly Version _version;

    public InstallingVm(string path, bool createShortcut, Version? version)
    {
        _path = path;
        _createShortcut = createShortcut;
        _version = version ?? HelperExtensions.EmptyVersion;

        StartInstallCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var installer = Locator.Current.GetRequiredService<IInstaller>();

            await installer.InstallAsync(path, _version, createShortcut, progress =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Progress = progress.Value * 100;
                    State = progress.State;
                });
            });
        });

        StartInstallCommand.Subscribe(async _ =>
        {
            await Task.Delay(1200);

            await HostScreen.Router.Navigate.Execute(new CompleteVm(_path)).FirstAsync();
        });
        
    }

    [Reactive]
    public double Progress
    {
        get; set;
    }

    [Reactive]
    public string State
    {
        get; set;
    } = null!;

    public ReactiveCommand<Unit,Unit> StartInstallCommand
    {
        get;
    }
}
