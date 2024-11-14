using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using RcsInstaller.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RcsInstaller.Vms;
public sealed class MaintenancePageVm: PageVm
{
    private readonly IInstaller _installer;
    private readonly IUpdater _updater;
    private readonly IRepair _repair;
    private readonly IAppRegistry _appRegistry;

    public MaintenancePageVm(IInstaller installer, IUpdater updater, IRepair repair, IAppRegistry appRegistry)
    {
        _installer = installer;
        _updater = updater;
        _repair = repair;
        _appRegistry = appRegistry;

        RepairCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _repair.RepairAsync(x => Dispatcher.UIThread.Post(() =>
            {
                Progress = x.Value * 100;

            }));
        });
    }

    public async Task InitVm()
    {

    }

    public ReactiveCommand<Unit, Unit> RepairCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> UninstallCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> UpdateCommand
    {
        get;
    }

    [Reactive]
    public Version CurrentVersion
    {
        get; set;
    }

    [Reactive]
    public Version LatestVersion
    {
        get; set;
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
    }
}
