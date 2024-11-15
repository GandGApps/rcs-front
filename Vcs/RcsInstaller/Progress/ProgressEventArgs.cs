using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Progress;
public sealed class ProgressEventArgs : EventArgs
{
    private readonly double _progress;

    public ProgressEventArgs(double progress)
    {
        _progress = progress;
    }

    public double Progress => _progress;

}