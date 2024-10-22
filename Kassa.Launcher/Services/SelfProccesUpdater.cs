using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Splat;

namespace Kassa.Launcher.Services;
internal sealed partial class SelfProccesUpdater(RepoInfo repoInfo) : ISelfUpdater, IEnableLogger
{

    public async Task<bool> HasUpdates(Action<double> progress)
    {
        var process = new Process();
        process.StartInfo.FileName = "Kassa.Launcher.AutoUpdater.exe";
        process.StartInfo.Arguments = $"check -i \"{AppDomain.CurrentDomain.BaseDirectory.Replace('\\','/')}\" -t \"{repoInfo.Token}\" -r \"{repoInfo.Repo}\" -b \"launcher\" -a \"LauncherUpdater\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        // Log proccess info
        this.Log().Info("Checking for updates: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

        process.Start();

        var line = process.StandardOutput.ReadLine();
        var lastLine = "0";
        // Checking for updates... x %

        var regex = ProgressPatern();

        while (line != null)
        {
            line = line.Trim();
            this.Log().Info(line);

            var match = regex.Match(line);

            if (match.Success)
            {
                var parsedDouble = double.Parse(match.Groups[1].Value);
                progress(parsedDouble);
            }
            lastLine = line ?? lastLine;
            line = process.StandardOutput.ReadLine();
        }

        await process.WaitForExitAsync();

        return lastLine == "1";
    }

    public Task Update()
    {
        var process = new Process();
        process.StartInfo.FileName = "Kassa.Launcher.AutoUpdater.exe";
        process.StartInfo.Arguments = $"update -i \"{AppDomain.CurrentDomain.BaseDirectory.Replace('\\', '/')}\" -t \"{repoInfo.Token}\" -r \"{repoInfo.Repo}\" -b \"launcher\" -a \"LauncherUpdater\" -p {Environment.ProcessId}";
        process.StartInfo.RedirectStandardOutput = true;

        // Log proccess info
        this.Log().Info("Checking for updates: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

        process.Start();

        return Task.CompletedTask;
    }

    [GeneratedRegex(@"(\d+)\s%")]
    private static partial Regex ProgressPatern();
}
