using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using RcsInstaller.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RcsInstaller.Vms;
public sealed class MaintenanceVm: PageVm
{
    private readonly IInstaller _installer;
    private readonly IUpdater _updater;
    private readonly IRepair _repair;

    public MaintenanceVm(IInstaller installer, IUpdater updater, IRepair repair)
    {
        _installer = installer;
        _updater = updater;
        _repair = repair;
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
}
