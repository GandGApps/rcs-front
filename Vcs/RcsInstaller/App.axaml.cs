using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using RcsInstaller.Services;
using Refit;
using Splat;
using System.IO;
using System.Runtime.CompilerServices;

namespace RcsInstaller;

public sealed partial class App : Application
{

    public static new App Current => Unsafe.As<App>(Application.Current)!;

    public override void Initialize()
    {
        Program.AddServicesForLocator();

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var result = Parser.Default.ParseArguments<UpdateOption>(desktop.Args ?? []);

            //open the registry and get the path of the installed app
#pragma warning disable CA1416 // Validate platform compatibility

            if(result.Value is null)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(LauncherConstants.RegistryKeyPath))
                {
                    var installLocation = key?.GetValue("InstallLocation") as string;

                    if (installLocation != null)
                    {
                        // Remove last part
                        installLocation = Path.GetDirectoryName(installLocation)!;
                        desktop.MainWindow = new MainWindow(installLocation);
                        return;
                    }
                }
            }

            
#pragma warning restore CA1416 // Validate platform compatibility

            desktop.MainWindow = new MainWindow(result.Value);
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void Exit()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Current.ApplicationLifetime!;

        lifetime.Shutdown();
    }
}