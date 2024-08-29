using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;

namespace Kassa.Avalonia.Services;
internal static class PresentationLayerServices
{
    internal static void RegisterDispatherAdapter()
    {
        ServiceLocatorBuilder.AddService<Shared.IDispatcher>(new DispatcherAdapter());
    }

    private sealed class DispatcherAdapter : Shared.IDispatcher
    {
        public void InvokeAsync(Action action) => Dispatcher.UIThread.InvokeAsync(action);
        public void InvokeAsync(Func<Task> action) => Dispatcher.UIThread.InvokeAsync(action);
    }
}


