using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using Kassa.BuisnessLogic.Edgar;
using Kassa.BuisnessLogic;
using Kassa.DataAccess.HttpRepository;
using Kassa.DataAccess;
using Kassa.RxUI;
using Kassa.Shared.ServiceLocator;
using Kassa.Shared;
using ReactiveUI;
using Splat;
using Microsoft.Extensions.Configuration;

namespace Kassa.Avalonia;
internal static class AvaloniaPresentationLayerServiceRegistration
{
    public static AppBuilder UseRcs(this AppBuilder appBuilder)
    {
        MainViewModel.RegsiterMainViewModel();

        ServiceLocatorBuilder.SetLocator();

        return appBuilder;
    }
}
