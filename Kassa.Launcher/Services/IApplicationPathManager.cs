using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;
public interface IApplicationPathManager
{
    public Task<string> GetApplicationPath();

    public Task SetApplicationPath(string path);
}
