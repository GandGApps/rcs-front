using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Octokit;

namespace Kassa.Launcher.AutoUpdater;
internal sealed class GitHubUpdater : IUpdater
{
    private readonly BaseOption _baseOption;
    private readonly GitHubClient _client;

    public GitHubUpdater(BaseOption repoInfo)
    {
        _baseOption = repoInfo;
        _client = new GitHubClient(new ProductHeaderValue(_baseOption.AppName))
        {
            Credentials = new Credentials(_baseOption.Token)
        };
    }

    public async Task<bool> CheckForUpdatesAsync(Action<double> progress)
    {
        var ownerRepo = _baseOption.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            ThrowHelper.ThrowArgumentException("Repository should be in the format 'owner/repo'.");
        }

        var owner = ownerRepo[0];
        var repoName = ownerRepo[1];

        var latestCommit = await _client.Repository.Commit.Get(owner, repoName, _baseOption.Branch);
        var tree = await _client.Git.Tree.GetRecursive(owner, repoName, latestCommit.Sha);

        var indexer = 0d;
        var totalFiles = tree.Tree.Count(t => t.Type == TreeType.Blob);
        var basePath = _baseOption.InstallPath;

        var changedFiles = tree.Tree
            .Where(t =>
            {
                if (t.Path.EndsWith("Kassa.Launcher.AutoUpdater"))
                {
                    return false;
                }
                
                progress(indexer++ / totalFiles);
                return t.Type == TreeType.Blob && (!FileExists(basePath, t.Path) || !CompareHashes(basePath, t.Path, t.Sha));
            })
            .ToList();

        progress(1.0);

        return changedFiles.Count > 0;
    }

    public async Task<bool> UpdateAsync(Action<double> progress)
    {
        var path = _baseOption.InstallPath;
        var ownerRepo = _baseOption.Repo.Split('/');
        if (ownerRepo.Length != 2)
        {
            ThrowHelper.ThrowArgumentException("Repository should be in the format 'owner/repo'.");
        }

        var owner = ownerRepo[0];
        var repoName = ownerRepo[1];
        var latestCommit = await _client.Repository.Commit.Get(owner, repoName, _baseOption.Branch);
        var tree = await _client.Git.Tree.GetRecursive(owner, repoName, latestCommit.Sha);
        var basePath = _baseOption.InstallPath;
        var changedFiles = tree.Tree
            .Where(t => !t.Path.EndsWith("Kassa.Launcher.AutoUpdater.exe") && t.Type == TreeType.Blob && (!FileExists(basePath, t.Path) || !CompareHashes(basePath, t.Path, t.Sha)))
            .ToList();

        var totalFiles = changedFiles.Count;

        for (var i = 0; i < totalFiles; i++)
        {
            var file = changedFiles[i];
            await DownloadAndUpdate(file.Path, file.Url, path);
            progress((i+1d) / totalFiles);
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
        // Построение правильного URL для скачивания
        var rawUrl = $"https://raw.githubusercontent.com/{_baseOption.Repo}/{_baseOption.Branch}/{filePath}";
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
}
