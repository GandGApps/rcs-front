using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruePath;

namespace RcsInstaller.Services;
public interface IUpdater
{
    public Task UpdateAsync(AbsolutePath path, Version version, Action<ProgressState> value);
}
