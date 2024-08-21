using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using Kassa.Wpf.Services;
using Splat;

namespace Kassa.Wpf;
internal static class PresentationLayerServices
{

    public static void RegisterDispatherAdapter()
    {
        RcsLocatorBuilder.AddSingleton<IDispatcher, DispatherAdapter>();
    }

}
