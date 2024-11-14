using System;
using System.Threading.Tasks;
using RcsInstaller.Services;
using TruePath;

namespace RcsInstaller.Vms;

public interface IRepair
{
    public Task RepairAsync(Action<ProgressState> value);
}