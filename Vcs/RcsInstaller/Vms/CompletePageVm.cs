using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RcsInstaller.Configurations;
using RcsInstaller.Progress;
using RcsInstaller.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TruePath;

namespace RcsInstaller.Vms;
public sealed class CompletePageVm : PageVm
{
    private readonly IAppRegistry _appRegistry;
    private readonly IRcsApi _rcsApi;
    private readonly ITargetAppRunner _targetAppRunner;
    private readonly IRepair _repair;
    private readonly IUpdater _updater;
    private readonly IRemover _remover;

    public CompletePageVm(AbsolutePath path, Version currentVersion, IAppRegistry appRegistry, IRcsApi rcsApi, ITargetAppRunner targetAppRunner, IUpdater updater, IRepair repair, IRemover remover)
    {
        _appRegistry = appRegistry;
        _rcsApi = rcsApi;
        _targetAppRunner = targetAppRunner;
        _repair = repair;
        _updater = updater;
        _remover = remover;

        CloseCommand = ReactiveCommand.Create(() =>
        {
            App.Exit();
        });

        StartAppCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _targetAppRunner.Run();
        });

        RepairCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Task RepairTask(ProgressCalback progress) => _repair.RepairAsync(x => progress(this, new(x)));
            
            var progressPageVm = new ProgressPageVm();

            await progressPageVm.StartTaskAndShowProgress(RepairTask);

            await HostScreen.Router.NavigateBack.Execute();

            await AccessAppInformationRegisty();
        });

        var canExecuteUpdateCommand = this.WhenAnyValue(x => x.IsUpdateButtonWorking, x => x.CurrentVersion, x => x.LatestVersion)
            .Select<(bool IsUpdateButtonWorking, Version CurrentVersion, Version? LatestVersion), bool>(x =>
            {
                if (x.IsUpdateButtonWorking)
                {
                    return false;
                }

                if (x.LatestVersion is null)
                {
                    return false;
                }

                return x.CurrentVersion < x.LatestVersion;
            });


        UpdateCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Task UpdateTask(ProgressCalback progress) => _updater.UpdateAsync(x => progress(this, new(x)));

            var progressPageVm = new ProgressPageVm();

            await progressPageVm.StartTaskAndShowProgress(UpdateTask);

            await HostScreen.Router.NavigateBack.Execute();

            await AccessAppInformationRegisty();
        }, canExecute: canExecuteUpdateCommand);

        CurrentVersion = currentVersion;

        this.WhenAnyValue(x => x.CurrentVersion, x => x.LatestVersion)
            .Select<(Version CurrentVersion, Version? LatestVersion), string>(x =>
            {
                if(x.LatestVersion is null)
                {
                    return "Поиск обновление...";
                }

                return x.CurrentVersion < x.LatestVersion ? "Обновить" : "Обновление не требуется";
            })
            .ToPropertyEx(this, x => x.UpdateButtonText);

        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Task RemoveTask(ProgressCalback progress) => _remover.RemoveAsync(x => progress(this, new(x)));

            var progressPageVm = new ProgressPageVm();

            await progressPageVm.StartTaskAndShowProgress(RemoveTask);

            App.Exit();
        });
    }

    private async Task AccessAppInformationRegisty()
    {
        CurrentVersion = (await _appRegistry.GetVersion())!;
    }

    public async Task CheckForUpdates()
    {
        IsUpdateButtonWorking = true;

        try
        {
            LatestVersion = await _rcsApi.GetVersion();
        }
        finally
        {
            IsUpdateButtonWorking = false;
        }
    }


    public ReactiveCommand<Unit, Unit> CloseCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> StartAppCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> RepairCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> UpdateCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    [Reactive]
    public Version CurrentVersion
    {
        get; set;
    }

    [Reactive]
    public Version? LatestVersion
    {
        get; set;
    }

    [Reactive]
    public bool IsUpdateButtonWorking
    {
        get; set;
    }

    public extern string UpdateButtonText
    {
        [ObservableAsProperty]
        get;
    }
}
