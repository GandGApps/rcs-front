using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;

internal sealed class RepoInfo
{

    public string Token
    {
        get;
    }

    public string Repo
    {
        get;
    }

    public string Branch
    {
        get;
    }

    public string AppName
    {
        get;
    }

    public RepoInfo(string token, string repo, string branch, string appName)
    {
        Token = token;
        Repo = repo;
        Branch = branch;
        AppName = appName;
    }

}
