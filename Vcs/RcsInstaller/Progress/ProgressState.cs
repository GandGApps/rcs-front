using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Progress;
public sealed record ProgressState(string State, double Value);