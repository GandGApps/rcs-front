using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.BuisnessLogic.Services;
internal class InitializableServiceFactory<T> : IInitializableServiceFactory<T> where T : class, IInitializableService
{
    private T? _service;

    public async ValueTask<T> GetService()
    {
        _service ??= Locator.Current.GetRequiredService<T>();

        if (_service.IsDisposed)
        {
            _service = null;
            _service = Locator.Current.GetRequiredService<T>();
        }

        if (_service.IsInitialized)
        {
            return _service;
        }

        await _service.Initialize();

        return _service;
    }

    public T GetNotInitializedService()
    {
        _service ??= Locator.Current.GetRequiredService<T>();

        if (_service.IsDisposed)
        {
            _service = null;
            _service = Locator.Current.GetRequiredService<T>();
        }

        return _service;
    }
}
