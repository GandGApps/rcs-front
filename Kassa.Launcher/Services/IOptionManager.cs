using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;
public interface IOptionManager
{
    public void SaveOption<T>(string key, T value);

    public T GetOption<T>(string key);
}
