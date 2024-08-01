using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Kassa.Launcher.AutoUpdater;
public abstract class RepoInfo
{
    [Option('t', "token", Required = true, HelpText = "GitHub token")]
    public string Token
    {
        get; set;
    } = string.Empty;

    [Option('r', "repo", Required = true, HelpText = "GitHub repository")]
    public string Repo
    {
        get; set;
    }

    [Option('b', "branch", Required = true, HelpText = "GitHub branch")]
    public string Branch
    {
        get; set;
    }

    [Option('a', "app", Required = false, HelpText = "App name")]
    public string AppName
    {
        get; set;
    } = "AutoUpdater";

    [Option('i', "install", Required = true, HelpText = "Install path")]
    public string InstallPath
    {
        get; set;
    }

}

[Verb("check")]
public sealed class CheckUpdatesOption: RepoInfo
{
}

[Verb("update")]
public sealed class UpdateOption: RepoInfo
{
    
}