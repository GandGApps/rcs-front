using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using Kassa.Wpf.Services;
using Splat;

namespace Kassa.Wpf;
internal static class SplatExtensions
{

    public static void AddDispatherAdapter(this IMutableDependencyResolver services)
    {
        services.RegisterConstant<IDispatcher>(new DispatherAdapter());
    }

}
