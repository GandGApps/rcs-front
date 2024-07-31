using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher;
public static class LauncherConstants
{
    public const string ApplicationPathKey = "KASSA_INSTALL_PATH";
    public const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + "RcsKassa";
}
