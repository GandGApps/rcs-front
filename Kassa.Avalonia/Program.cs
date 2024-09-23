using System;
using System.Globalization;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Kassa.RxUI;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;
using Serilog;
using Splat;

namespace Kassa.Avalonia;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            // This is end.
            // there's nothing we can do.
            LogHost.Default.Fatal((Exception)e.ExceptionObject, "An unhandled exception occurred");

            // Dans mon esprit tout divague, je me perds dans tes yeux
            // Je me noie dans la vague de ton regard amoureux
            // Je ne veux que ton âme divaguant sur ma peau
            // Une fleur, une femme dans ton cœur Roméo
            // Je ne suis que ton nom, le souffle lancinant
            // De nos corps dans le sombre animés lentement
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            var mainViewModel = RcsLocator.GetRequiredService<MainViewModel>();
            var exceptions = e.Exception.InnerExceptions;
            var handled = false;

            foreach(var exception in exceptions)
            {
                handled = mainViewModel.TryHandleUnhandled("TaskScheduler.UnobservedTaskException", exception);

                if (!handled)
                {
                    return;
                }
            }

            e.SetObserved();
        };


        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogHost.Default.Fatal(e, "An unhandled exception occurred");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .UseRcs()
            .UseDirect2D1()
            .LogToTrace();
}