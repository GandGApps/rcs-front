using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Services;
public interface ITargetAppRunner
{
    public Task Run();
}
