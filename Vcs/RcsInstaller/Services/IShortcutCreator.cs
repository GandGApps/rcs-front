using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Services;
public interface IShortcutCreator
{
    public Task CreateSrotcut(string path, string name);
}
