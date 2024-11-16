using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using RcsInstaller.Configs;
using RcsInstaller.Configurations;
using RcsInstaller.Services;
using RcsInstaller.Vms;
using Refit;
using Splat;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using TruePath;

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

    public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

    public static string CurrentPath => Path.Combine(BasePath, $"{nameof(RcsInstaller)}.exe");
    public static AbsolutePath CurrentPathAbsolute => new(CurrentPath);

    public override void Initialize()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateEmptyApplicationBuilder(null);

        if(OperatingSystem.IsWindows())
        {
            builder.Logging.AddEventLog();
        }

        builder.Configuration.AddEmbeddedJsonFile("appsettings.json");
#if DEBUG
        builder.Configuration.AddEmbeddedJsonFile("appsettings.Debug.json");
#endif

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        builder.Services.Configure<ApiConfiguration>(builder.Configuration.GetSection(nameof(ApiConfiguration)));
        builder.Services.Configure<TargetAppInfo>(builder.Configuration.GetSection(nameof(TargetAppInfo)));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        builder.Services.AddApi<IRcsApi>();

        builder.Services.AddSingleton<RcsInstallerJson>();
        builder.Services.AddSingleton<IInstaller, RcsInstallerJson>(sp => sp.GetRequiredService<RcsInstallerJson>());
        builder.Services.AddSingleton<IUpdater>(sp => sp.GetRequiredService<RcsInstallerJson>());
        builder.Services.AddSingleton<IRepair>(sp => sp.GetRequiredService<RcsInstallerJson>());
        builder.Services.AddSingleton<IRemover, SimpleRemover>();
        builder.Services.AddSingleton<IShortcutCreator, WndShortcutCreator>();
        builder.Services.AddSingleton<IAppRegistry, WndAppRegistry>();

        Host = builder.Build();

        AvaloniaXamlLoader.Load(this);
    }

    [STAThread]
    public async override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var result = Parser.Default.ParseArguments<UpdateOption>(desktop.Args ?? []);


            if(result.Value is null)
            {
                var appRegistry = Host.Services.GetRequiredService<IAppRegistry>();

                var properties = await appRegistry.GetProperties();

                if (properties is AppRegistryProperties appRegistryProperties)
                {
                    desktop.MainWindow = new MainWindow(appRegistryProperties.Path);
                    return;
                }
            }

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