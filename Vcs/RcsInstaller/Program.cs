using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using RcsInstaller.Services;
using Splat;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RcsInstaller;

public static class Program
{
    [STAThread]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UpdateOption))]
    public static void Main(string[] args)
    {
        var app = BuildAvaloniaApp();
            
        var result = app.StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();

}
