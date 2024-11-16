using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Configs;
public sealed class ApiConfiguration(string baseAddress)
{
    public string BaseAddress => baseAddress;
}

