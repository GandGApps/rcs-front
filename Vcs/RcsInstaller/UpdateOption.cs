using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller;

[Verb("update", HelpText = "Update existing application")]
public sealed class UpdateOption
{
    [Option('p', "path", Required = true, HelpText = "Path to install")]
    public required string Path
    {
        get;  set;
    }

    [Option('v', "version", Required = true, HelpText = "Version to install")]
    public required string Version
    {
        get; set;
    }
}
