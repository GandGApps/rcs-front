using KassaLauncher.Services;
using Octokit;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Vms;

public sealed class InstallerVm : BaseVm
{
    private readonly IUpdater _updater;

    [Reactive]
    public string? Status
    {
        get; private set;
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

    [Reactive]
    public bool IsInstalling
    {
        get; private set;
    }

    [Reactive]
    public double Progress
    {
        get; private set;
    }

    public ReactiveCommand<Unit, Unit> InstallCommand
    {
        get;
    }

    public InstallerVm(IUpdater updater)
    {
        _updater = updater;

        var validator = this.WhenAnyValue(x => x.InstallPath)
            .Select(path => !string.IsNullOrEmpty(path));

        InstallCommand = ReactiveCommand.CreateFromTask(InstallOrUpdateAsync, validator);
    }

    public async Task InstallOrUpdateAsync()
    {
        var isInstalled = await _updater.IsInstalled(InstallPath);
        if (!isInstalled)
        {
            Status = "Установка начата...";
            IsInstalling  = true;
            await _updater.InstallAsync(InstallPath, IsShortcutNeeded, UpdateProgress);
            Status = "Установка завершена. Все файлы установлены.";
            await Task.Delay(2000);
            IsInstalling = false;
        }
        else
        {
            IsInstalling = true;
            var hasUpdates = await _updater.UpdateAsync(UpdateProgress);
            if (hasUpdates)
            {
                Status = "Обновление завершено. Все файлы обновлены.";
            }
            else
            {
                Status = "У вас установлена последняя версия.";
            }

            await Task.Delay(2000);

            IsInstalling = false;
        }
    }

    private void UpdateProgress(double progress)
    {
        Progress = progress * 100;
    }
}