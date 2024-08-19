using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;
public interface ISelfUpdater
{
    public Task<bool> HasUpdates(Action<double> progress);

    public Task Update();
}
