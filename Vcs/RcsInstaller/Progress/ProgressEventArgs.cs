using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Progress;
public sealed class ProgressEventArgs : EventArgs
{
    private readonly ProgressState _progressState;

    public ProgressEventArgs(double progress): this(new ProgressState(string.Empty, progress))
    {
    }

    public ProgressEventArgs(ProgressState progressState)
    {
        _progressState = progressState;
    }


    public double Progress => _progressState.Value;
    public ProgressState ProgressState => _progressState;

}