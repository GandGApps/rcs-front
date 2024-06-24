using Kassa.Launcher.Services;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Services;

internal sealed class GitHubUpdater : IUpdater
{
    private readonly RepoInfo _repoInfo;
    private readonly GitHubClient _client;
    private readonly IDesktopShortcutCreator _shortcutCreator;

    public GitHubUpdater(RepoInfo repoInfo, IDesktopShortcutCreator shortcutCreator)
    {
        _repoInfo = repoInfo;
        _client = new GitHubClient(new ProductHeaderValue(_repoInfo.AppName))
        {
            Credentials = new Credentials(_repoInfo.Token)
        };
        _shortcutCreator = shortcutCreator;
    }

    public async Task<bool> CheckForUpdatesAsync(Action<double> progress)
    {
        var ownerRepo = _repoInfo.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            throw new ArgumentException("Repository should be in the format 'owner/repo'.");
        }

        var owner = ownerRepo[0];
        var repoName = ownerRepo[1];
        
        var latestCommit = await _client.Repository.Commit.Get(owner, repoName, _repoInfo.Branch);
        var tree = await _client.Git.Tree.GetRecursive(owner, repoName, latestCommit.Sha);

        var indexer = 0d;
        var totalFiles = tree.Tree.Count(t => t.Type == TreeType.Blob); 

        var changedFiles = tree.Tree
            .Where(t =>
            {
                progress(indexer / totalFiles);
                return t.Type == TreeType.Blob && (!FileExists(t.Path) || !CompareHashes(t.Path, t.Sha));
            })
            .ToList();

        progress(1.0); 

        return changedFiles.Count > 0;
    }

    public ValueTask<bool> IsInstalled(string path)
    {
        var fullPath = Environment.GetEnvironmentVariable("KASSA_INSTALL_PATH", EnvironmentVariableTarget.User);

        if(string.IsNullOrWhiteSpace(fullPath))
        {
            return new(false);
        }

        if(!Directory.Exists(fullPath))
        {
            return new(false);
        }

        if(Directory.GetFiles(fullPath).Length == 0)
        {
            return new(false);
        }

        return new(fullPath.Equals(path, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task InstallAsync(string path, bool createShortcut, Action<double> progress)
    {
        var ownerRepo = _repoInfo.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            throw new ArgumentException("Repository should be in the format 'owner/repo'.");
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
    }

    public async Task<bool> UpdateAsync(Action<double> progress)
    {
        var path = Environment.GetEnvironmentVariable("KASSA_INSTALL_PATH", EnvironmentVariableTarget.User)!;
        var ownerRepo = _repoInfo.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            throw new ArgumentException("Repository should be in the format 'owner/repo'.");
        }

        var owner = ownerRepo[0];
        var repoName = ownerRepo[1];
        var latestCommit = await _client.Repository.Commit.Get(owner, repoName, _repoInfo.Branch);
        var tree = await _client.Git.Tree.GetRecursive(owner, repoName, latestCommit.Sha);
        var changedFiles = tree.Tree
            .Where(t => t.Type == TreeType.Blob && (!FileExists(t.Path) || !CompareHashes(t.Path, t.Sha)))
            .ToList();

        var totalFiles = changedFiles.Count;

        for (int i = 0; i < totalFiles; i++)
        {
            var file = changedFiles[i];
            await DownloadAndUpdate(file.Path, file.Url, path);
            progress((double)(i + 1) / totalFiles);
        }

        return totalFiles > 0;
    }

    private bool FileExists(string filePath)
    {
        var fullPath = Path.Combine(Environment.GetEnvironmentVariable("KASSA_INSTALL_PATH", EnvironmentVariableTarget.User)!, filePath);
        return File.Exists(fullPath);
    }

    private static bool CompareHashes(string filePath, string gitSha)
    {
        var fullPath = Path.Combine(Environment.GetEnvironmentVariable("KASSA_INSTALL_PATH", EnvironmentVariableTarget.User)!, filePath);
        if (!File.Exists(fullPath))
        {
            return false;
        }

        byte[] fileContent = File.ReadAllBytes(fullPath);
        var gitObjectHeader = $"blob {fileContent.Length}\0";
        byte[] headerBytes = Encoding.UTF8.GetBytes(gitObjectHeader);

        using var sha1 = SHA1.Create();
        sha1.TransformBlock(headerBytes, 0, headerBytes.Length, headerBytes, 0);
        sha1.TransformFinalBlock(fileContent, 0, fileContent.Length);
        var fileHash = BitConverter.ToString(sha1.Hash).Replace("-", "").ToLower();

        return fileHash == gitSha;
    }

    private async Task DownloadAndUpdate(string filePath, string downloadUrl, string installPath)
    {

        // Построение правильного URL для скачивания
        var rawUrl = $"https://raw.githubusercontent.com/{_repoInfo.Repo}/{_repoInfo.Branch}/{filePath}";
        var fullPath = Path.Combine(installPath, filePath);
        var fileDirectory = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(fileDirectory))
        {
            Directory.CreateDirectory(fileDirectory);
        }

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(rawUrl);
            response.EnsureSuccessStatusCode();

            using (var fileStream = new FileStream(fullPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(fileStream);
            }
        }
    }
}
