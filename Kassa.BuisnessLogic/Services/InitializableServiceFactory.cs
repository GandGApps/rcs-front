using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace Kassa.BuisnessLogic.Services;
internal sealed class InitializableServiceFactory<T> : IInitializableServiceFactory<T>, IEnableLogger where T : class, IInitializableService
{
    private T? _service;

    public async ValueTask<T> GetService()
    {
        if (_service == null)
        {
            this.Log().Info("Initialize new instance " + typeof(T));
        }

        _service ??= RcsKassa.ServiceProvider.GetRequiredService<T>();

        if (_service.IsDisposed)
        {
            _service = null;
            _service = RcsKassa.ServiceProvider.GetRequiredService<T>();
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
        if (_service == null)
        {
            this.Log().Info("Return new not initialized instance of service" + typeof(T));
        }

        _service ??= RcsKassa.ServiceProvider.GetRequiredService<T>();

        if (_service.IsDisposed)
        {
            this.Log().Info("Return new not initialized instance of service " + typeof(T));
            
            _service = null;
            _service = RcsKassa.ServiceProvider.GetRequiredService<T>();
        }

        return _service;
    }
}
