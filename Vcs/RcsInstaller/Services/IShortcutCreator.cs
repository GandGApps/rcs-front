using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruePath;

namespace RcsInstaller.Services;
public interface IShortcutCreator
{
    public Task CreateSrotcut(AbsolutePath path, string name);
}
