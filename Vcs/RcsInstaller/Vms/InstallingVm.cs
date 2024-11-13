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
using TruePath;

namespace RcsInstaller.Vms;
public sealed class InstallingVm : PageVm
{
    private readonly AbsolutePath _path;
    private readonly bool _createShortcut;
    private readonly Version _version;
    private readonly IInstaller _installer;

    public InstallingVm(string path, bool createShortcut, Version? version, IInstaller installer)
    {
        _path = new(path);
        _createShortcut = createShortcut;
        _version = version ?? HelperExtensions.EmptyVersion;
        _installer = installer;

        StartInstallCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _installer.InstallAsync(_path, _version, _createShortcut, progress =>
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

            await HostScreen.Router.Navigate.Execute(App.CreateInstance<CompleteVm>(_path)).FirstAsync();
        });

        UpdateCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _installer.UpdateAsync(_path, _version, progress =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Progress = progress.Value * 100;
                    State = progress.State;
                });
            });
        });

        UpdateCommand.Subscribe(async _ =>
        {
            await Task.Delay(1200);

            await HostScreen.Router.Navigate.Execute(App.CreateInstance<CompleteVm>(_path)).FirstAsync();
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

    public ReactiveCommand<Unit, Unit> UpdateCommand
    {
        get;
    }
}
