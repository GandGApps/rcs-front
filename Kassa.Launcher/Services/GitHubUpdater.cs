using CommunityToolkit.Diagnostics;
using Kassa.Launcher;
using Kassa.Launcher.Services;
using Microsoft.Win32;
using Octokit;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Services;

internal sealed class GitHubUpdater : IUpdater, IInstaller, IEnableLogger
{
    private readonly RepoInfo _repoInfo;
    private readonly GitHubClient _client;
    private readonly IDesktopShortcutCreator _shortcutCreator;

    private readonly IApplicationPathManager _pathManager;

    public GitHubUpdater(RepoInfo repoInfo, IDesktopShortcutCreator shortcutCreator, IApplicationPathManager applicationPathManager)
    {
        _repoInfo = repoInfo;
        _client = new GitHubClient(new ProductHeaderValue(_repoInfo.AppName))
        {
            Credentials = new Credentials(_repoInfo.Token)
        };
        _shortcutCreator = shortcutCreator;
        _pathManager = applicationPathManager;
    }

    public async Task<bool> CheckForUpdatesAsync(Action<double> progress)
    {
        var ownerRepo = _repoInfo.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            ThrowHelper.ThrowArgumentException("Repository should be in the format 'owner/repo'.");
        }

        var owner = ownerRepo[0];
        var repoName = ownerRepo[1];

        var latestCommit = await _client.Repository.Commit.Get(owner, repoName, _repoInfo.Branch);
        var tree = await _client.Git.Tree.GetRecursive(owner, repoName, latestCommit.Sha);

        var indexer = 0d;
        var totalFiles = tree.Tree.Count(t => t.Type == TreeType.Blob);
        var basePath = await _pathManager.GetApplicationPath();

        var changedFiles = tree.Tree
            .Where(t =>
            {
                progress(indexer / totalFiles);
                return t.Type == TreeType.Blob && (!FileExists(basePath, t.Path) || !CompareHashes(basePath, t.Path, t.Sha));
            })
            .ToList();

        progress(1.0);

        return changedFiles.Count > 0;
    }

    public async ValueTask<bool> IsInstalled(string path)
    {
        var fullPath = await _pathManager.GetApplicationPath();

        if (string.IsNullOrWhiteSpace(fullPath))
        {
            return false;
        }

        if (!Directory.Exists(fullPath))
        {
            return false;
        }

        if (Directory.GetFiles(fullPath).Length == 0)
        {
            return false;
        }

        return fullPath.Equals(path, StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task InstallAsync(string path, bool createShortcut, Action<double> progress)
    {
        var ownerRepo = _repoInfo.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            ThrowHelper.ThrowArgumentException("Repository should be in the format 'owner/repo'.");
        }

        var owner = ownerRepo[0];
        var repoName = ownerRepo[1];
        var latestCommit = await _client.Repository.Commit.Get(owner, repoName, _repoInfo.Branch);
        var tree = await _client.Git.Tree.GetRecursive(owner, repoName, latestCommit.Sha);
        var files = tree.Tree.Where(t => t.Type == TreeType.Blob).ToList();

        var totalFiles = files.Count;

        for (var i = 0; i < totalFiles; i++)
        {
            var file = files[i];
            await DownloadAndUpdate(file.Path, file.Url, path);
            progress((double)(i + 1) / totalFiles);
        }

        if (createShortcut)
        {
            var exePath = Path.Combine(path, "Kassa.Wpf.exe");
            await _shortcutCreator.CreateShortcutAsync(exePath, _repoInfo.AppName);
        }

        await _pathManager.SetApplicationPath(path);


#pragma warning disable CA1416 // Validate platform compatibility
        using var key = Registry.LocalMachine.CreateSubKey(LauncherConstants.RegistryKeyPath);
        if (key != null)
        {
            key.SetValue("DisplayName", "Супер мега касса с кодовым именем RCS");
            key.SetValue("DisplayVersion", "0.4.10");
            key.SetValue("Publisher", "Gang bang");
            key.SetValue("InstallLocation", Path.Combine(path, "Kassa.Wpf.exe"));
            key.SetValue("UninstallString", Path.Combine(App.BasePath, "Kassa.Launcher.exe --remove"));
            key.SetValue("NoModify", 1, RegistryValueKind.DWord);
            key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
        }
#pragma warning restore CA1416 // Validate platform compatibility
    }

    public async Task<bool> UpdateAsync(Action<double> progress)
    {
        var path = await _pathManager.GetApplicationPath();
        var ownerRepo = _repoInfo.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            ThrowHelper.ThrowArgumentException("Repository should be in the format 'owner/repo'.");
        }

        var owner = ownerRepo[0];
        var repoName = ownerRepo[1];
        var latestCommit = await _client.Repository.Commit.Get(owner, repoName, _repoInfo.Branch);
        var tree = await _client.Git.Tree.GetRecursive(owner, repoName, latestCommit.Sha);
        var basePath = await _pathManager.GetApplicationPath();
        var changedFiles = tree.Tree
            .Where(t => t.Type == TreeType.Blob && (!FileExists(basePath, t.Path) || !CompareHashes(basePath, t.Path, t.Sha)))
            .ToList();

        var totalFiles = changedFiles.Count;

        for (var i = 0; i < totalFiles; i++)
        {
            var file = changedFiles[i];
            await DownloadAndUpdate(file.Path, file.Url, path);
            progress((double)(i + 1) / totalFiles);
        }

        return totalFiles > 0;
    }

    private static bool FileExists(string basePath, string path) => File.Exists(Path.Combine(basePath, path));

    private static bool CompareHashes(string basePath, string filePath, string gitSha)
    {
        var fullPath = Path.Combine(basePath, filePath);
        if (!File.Exists(fullPath))
        {
            return false;
        }

        var fileContent = File.ReadAllBytes(fullPath);
        var gitObjectHeader = $"blob {fileContent.Length}\0";
        var headerBytes = Encoding.UTF8.GetBytes(gitObjectHeader);

        using var sha1 = SHA1.Create();
        sha1.TransformBlock(headerBytes, 0, headerBytes.Length, headerBytes, 0);
        sha1.TransformFinalBlock(fileContent, 0, fileContent.Length);
        var fileHash = BitConverter.ToString(sha1.Hash).Replace("-", "").ToLower();

        return fileHash == gitSha;
    }

    private async Task DownloadAndUpdate(string filePath, string downloadUrl, string installPath)
    {

        this.Log().Debug($"Downloading {filePath} from {downloadUrl}");

        const int maxRetries = 3;
        var currentRetry = 0;

    Download:
        try
        {
            // Построение правильного URL для скачивания
            var rawUrl = $"https://raw.githubusercontent.com/{_repoInfo.Repo}/{_repoInfo.Branch}/{filePath}";
            var fullPath = Path.Combine(installPath, filePath);
            var fileDirectory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(rawUrl);
            response.EnsureSuccessStatusCode();

            using var fileStream = new FileStream(fullPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fileStream);
        }
        catch (TaskCanceledException e)
        {
            if (currentRetry < maxRetries)
            {
                currentRetry++;
                this.Log().Error(e, $"Download failed. Retrying {currentRetry} of {maxRetries}");
                goto Download;
            }
            else
            {
                this.Log().Error(e, $"Download failed after {maxRetries} retries");
                throw;
            }
        }
        catch (Exception e)
        {
            this.Log().Error(e, "Download failed");
            throw;
        }


    }
}
