using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.AutoUpdater;
public interface IUpdater
{
    public Task<bool> CheckForUpdatesAsync(Action<double> progress);
    public Task<bool> UpdateAsync(Action<double> progress);
}
