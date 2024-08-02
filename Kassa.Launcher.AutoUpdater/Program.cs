using CommandLine;
using Kassa.Launcher.AutoUpdater;

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
        Console.WriteLine("0");
    }
    else
    {
        Console.WriteLine("1");
    }
}

static async Task Update(UpdateOption options)
{
    var updater = new GitHubUpdater(options);

    await updater.UpdateAsync(progress =>
    {
        Console.WriteLine($"Checking for updates... {progress:P0}");
    });

    Console.WriteLine("1");
}