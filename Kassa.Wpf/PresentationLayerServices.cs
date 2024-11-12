using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using Kassa.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace Kassa.Wpf;
internal static class PresentationLayerServices
{

    public static void AddDispatherAdapter(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher, DispatherAdapter>();
    }

    public static void AddHostedWpf(this IServiceCollection services)
    {
        services.AddHostedService<HostedWpf>();
    }
}
