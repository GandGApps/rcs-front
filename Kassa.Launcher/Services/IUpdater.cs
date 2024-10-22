using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;

public interface IUpdater
{
    public Task<bool> CheckForUpdatesAsync(Action<double> progress);
    public ValueTask<bool> IsInstalled(string path);
    public Task<bool> UpdateAsync(Action<double> progress);
}
