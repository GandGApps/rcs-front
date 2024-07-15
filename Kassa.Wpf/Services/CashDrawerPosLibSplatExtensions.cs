using System;
using System.Collections.Generic;
using System.IO.Ports;
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
            case CashDrawerPosLib.RawSerialPort:
                var rawBytesString = config.GetValue($"{nameof(RawSerialPort)}.RawBytes", "00")!;

                try
                {
                    var rawBytes = Convert.FromHexString(rawBytesString);
                }
                catch(Exception exc)
                {
                    LogHost.Default.Error(exc, $"Error parsing {nameof(RawSerialPort)}.RawBytes");
                    return;
                }

                var port = config.GetValue<string>($"{nameof(RawSerialPort)}.Port")!;

                if (!SerialPort.GetPortNames().Any(x => x == port))
                {
                    LogHost.Default.Error($"Port {port} not found");
                    return;
                }

                break;
            default:
                break;
        }

        LogHost.Default.Info($"Using CashDrawerPosLib: {cashDrawerPosLib}");
    }

}
