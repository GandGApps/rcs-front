using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.Wpf.Services;
internal static class CashDrawerPosLibSplatExtensions
{

    public static void AddCashDrawerPosLib(this IMutableDependencyResolver services, IConfiguration config)
    {

        var cashDrawerPosLibString = config.GetValue<string>(nameof(CashDrawerPosLib));

        var cashDrawerPosLib = Enum.TryParse<CashDrawerPosLib>(cashDrawerPosLibString, true, out var pos) ? pos : CashDrawerPosLib.WndPosLib;

        switch (cashDrawerPosLib)
        {

            case CashDrawerPosLib.WndPosLib:
                services.RegisterConstant<ICashDrawer>(new WndPosCashDrawer());
                break;
            default:
                break;
        }

        LogHost.Default.Info($"Using CashDrawerPosLib: {cashDrawerPosLib}");
    }

}
