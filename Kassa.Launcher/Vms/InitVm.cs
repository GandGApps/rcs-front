using Avalonia.Threading;
using Kassa.Launcher.Services;
using KassaLauncher.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Vms;

public sealed class InitVm : BaseVm
{

    private readonly IUpdater _updater;
    private readonly IInstaller _installer;

    [Reactive]
    public bool IsInstalled
    {
        get; private set;
    }

    [Reactive]
    public double Progress
    {
        get; private set;
    }

    [Reactive]
    public string? Status
    {
        get; private set;
    }

    public InitVm(IUpdater updater, IInstaller installer)
    {
        _updater = updater;
        _installer = installer;
    }

    public async Task InitAsync()
    {
        var installedPath = Environment.GetEnvironmentVariable("KASSA_INSTALL_PATH", EnvironmentVariableTarget.User);

        if (string.IsNullOrEmpty(installedPath))
        {
            Dispatcher.UIThread.Invoke(() => IsInstalled = false);
            
        }
        else
        {
            var result = await _updater.IsInstalled(installedPath); 
            Dispatcher.UIThread.Invoke(() => IsInstalled = result);
        }


        if (!IsInstalled)
        {
            HostScreen.Router.Navigate.Execute(new InstallerVm(_updater, _installer));
        }
        else
        {
            Dispatcher.UIThread.Invoke(() => Status = "Проверка обновлений...");
            var hasUpdates = await _updater.CheckForUpdatesAsync(progress => Dispatcher.UIThread.Invoke(() => Progress = progress * 100));
            if (hasUpdates)
            {
                
                Dispatcher.UIThread.Invoke(() => Status = "Загрузка обновлений...");

                await _updater.UpdateAsync(progress => Dispatcher.UIThread.Invoke(() => Progress = progress * 100));

                await Task.Delay(2000);

                Dispatcher.UIThread.Invoke(() => Status = "Обновление загружено");
            }

            await Task.Delay(2000);

            Dispatcher.UIThread.Invoke(() => HostScreen.Router.Navigate.Execute(new LaunchAppVm()));
        }
    }
}
