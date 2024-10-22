using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

namespace Kassa.Launcher.Services;
internal sealed class PathConstantMaanager(string path): IApplicationPathAccessor
{
    private readonly string _path = path;

    public Task<string> GetApplicationPath()
    {
        return Task.FromResult(_path);
    }
}