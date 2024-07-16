using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Kassa.Shared;

namespace Kassa.Wpf.Services;
internal sealed class DispatherAdapter : IDispatcher
{
    public void InvokeAsync(Action action) => Dispatcher.CurrentDispatcher.InvokeAsync(action);
    public void InvokeAsync(Func<Task> action) => Dispatcher.CurrentDispatcher.InvokeAsync(action);
}
