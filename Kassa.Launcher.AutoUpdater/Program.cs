using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using Kassa.Launcher.AutoUpdater;



internal static class Program
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(RepoInfo))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CheckUpdatesOption))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UpdateOption))]
    private static async Task Main(string[] args)
    {
        var parsed = Parser.Default.ParseArguments<CheckUpdatesOption, UpdateOption>(args);

        await parsed.WithParsedAsync<CheckUpdatesOption>(CheckUpdates);

        await parsed.WithParsedAsync<UpdateOption>(Update);

        static async Task CheckUpdates(CheckUpdatesOption options)
        {
            var updater = new GitHubUpdater(options);

            var has = await updater.CheckForUpdatesAsync(progress =>
            {
                Console.WriteLine($"Checking for updates... {progress:P0}");
            });

            if (has)
            {
                Console.WriteLine("1");
            }
            else
            {
                Console.WriteLine("0");
            }
        }

        static async Task Update(UpdateOption options)
        {
            var updater = new GitHubUpdater(options);

            try
            {
                if (options.ProcessId != 0)
                {
                    var process = Process.GetProcessById(options.ProcessId);

                    await process.WaitForExitAsync();
                }
                
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Proccess not found");
            }


            await updater.UpdateAsync(progress =>
            {
                Console.WriteLine($"Installing... {progress:P0}");
            });

            Console.WriteLine("1");
        }
    }
}