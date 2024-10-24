using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Kassa.Launcher;

public class KassaLauncherOption
{
    [Option('p', "path",  Required = true, HelpText = "path to target application")]
    public required string Path
    {
        get; set;
    }

    [Option('r', "remove", Required = false, HelpText = "command to remove app")]
    public bool Remove
    {
        get; set;
    }
}
