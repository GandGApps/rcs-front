using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.Shared;
public sealed class ScopeActivator: IServiceScope, IDisposable, IAsyncDisposable
{
    private readonly IServiceScope _serviceScope;

    public ScopeActivator(IServiceScope serviceScope)
    {
        _serviceScope = serviceScope;
    }

    public async ValueTask Activate()
    {
        var services = _serviceScope.ServiceProvider.GetServices<IInitializable>();

        foreach(var service in services)
        {
            await service.Initialize();
        }
    }

    public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

    public void Dispose() => _serviceScope.Dispose();

    public ValueTask DisposeAsync()
    {
        if (_serviceScope is IAsyncDisposable ad)
        {
            return ad.DisposeAsync();
        }
        _serviceScope.Dispose();


        return default;
    }
}
