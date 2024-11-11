using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.Shared;
public static class InitializableServicesRegsitrationExtensions
{

    public static void AddScopedInitializablesFromServiceCollection(this IServiceCollection services)
    {
        var initializables = services.Where(IsScopedIInitializable).ToImmutableArray();

        foreach (var service in initializables)
        {
            services.AddScoped(typeof(IInitializable), provider => provider.GetRequiredService(service.ServiceType));
        }
    }

    private static bool IsScopedIInitializable(this ServiceDescriptor serviceDescriptor)
    {
        return serviceDescriptor.Lifetime == ServiceLifetime.Scoped && serviceDescriptor.ImplementationType?.IsAssignableTo(typeof(IInitializable)) == true;
    }
}
