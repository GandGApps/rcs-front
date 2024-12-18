﻿using Avalonia.Threading;
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
public sealed class InstallingPageVm : PageVm
{
    private readonly AbsolutePath _path;
    private readonly bool _createShortcut;
    private readonly Version _version;
    private readonly IInstaller _installer;
    private readonly IUpdater _updater;

    public InstallingPageVm(string path, bool createShortcut, Version? version, IInstaller installer, IUpdater updater)
    {
        _path = new(path);
        _createShortcut = createShortcut;
        _version = version ?? HelperExtensions.EmptyVersion;
        _installer = installer;
        _updater = updater;

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

            await HostScreen.Router.Navigate.Execute(App.CreateInstance<CompletePageVm>(_path, _version)).FirstAsync();
        });

        UpdateCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _updater.UpdateAsync(progress =>
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

            await HostScreen.Router.Navigate.Execute(App.CreateInstance<CompletePageVm>(_path, _version)).FirstAsync();
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
