using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using RcsInstaller.Dto;
using RcsInstaller.Services;
using Refit;
using Splat;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace RcsInstaller;

public sealed partial class App : Application
{
    /// <summary>
    /// I understand that the host does not typically use this approach, and the host will "contain" the app, not the other way around.
    /// However, <see cref="Program.BuildAvaloniaApp"/> is needed for design-time purposes, so I need to store it here.
    /// If I build the host in the <see cref="Program.Main"/> method, many pages will break.
    /// </summary>
    public static IHost Host { get; private set; } = null!;
    public static new App Current => Unsafe.As<App>(Application.Current)!;

    public override void Initialize()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateEmptyApplicationBuilder(null);

#pragma warning disable CA1416 // Validate platform compatibility
        builder.Logging.AddEventLog();
#pragma warning restore CA1416 // Validate platform compatibility

        builder.Configuration.AddEmbeddedJsonFile("appsettings.json");

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        builder.Services.Configure<ApiConfiguration>(builder.Configuration);
        builder.Services.Configure<TargetAppInfo>(builder.Configuration);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        builder.Services.AddApi<IRcsApi>();
        builder.Services.AddSingleton<IInstaller, RcsInstallerJson>();
        builder.Services.AddSingleton<IUpdater>(sp => sp.GetRequiredService<RcsInstallerJson>());
        builder.Services.AddSingleton<IShortcutCreator, WndShortcutCreator>();

        Host = builder.Build();

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
                using var key = Registry.LocalMachine.OpenSubKey(LauncherConstants.RegistryKeyPath);
                if (key?.GetValue("InstallLocation") is string installLocation)
                {
                    // Remove last part
                    installLocation = Path.GetDirectoryName(installLocation)!;
                    desktop.MainWindow = new MainWindow(installLocation);
                    return;
                }
            }

            
#pragma warning restore CA1416 // Validate platform compatibility

            desktop.MainWindow = new MainWindow(result.Value);
        }

        base.OnFrameworkInitializationCompleted();
    }


    public static T CreateInstance<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
    {
        return ActivatorUtilities.CreateInstance<T>(Host.Services);
    }

    public static T CreateInstance<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(params object[] args)
    {
        return ActivatorUtilities.CreateInstance<T>(Host.Services, args);
    }

    public static void Exit()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Current.ApplicationLifetime!;

        lifetime.Shutdown();
    }
}