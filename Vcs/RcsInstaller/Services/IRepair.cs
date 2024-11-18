using System;
using System.Threading.Tasks;
using RcsInstaller.Progress;
using TruePath;

namespace RcsInstaller.Services;

public interface IRepair
{
    public Task RepairAsync(Action<ProgressState> value);
}