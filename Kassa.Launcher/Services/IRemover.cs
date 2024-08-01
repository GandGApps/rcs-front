using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;
public interface IRemover
{
    public Task RemoveAsync(Action<double> progress);
}
