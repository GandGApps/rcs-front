using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.Wpf.Services;
internal static class MscReaderLibSplatExtensions
{
    public static void AddMsrReaderLib(this IMutableDependencyResolver services, IConfiguration configuration)
    {
        var msrReaderLibString = configuration.GetValue<string>(nameof(MsrReaderLib));
        var msrReaderLib = Enum.TryParse<MsrReaderLib>(msrReaderLibString, true, out var pos) ? pos : MsrReaderLib.MsrPos;

        switch (msrReaderLib)
        {
            case MsrReaderLib.MsrPos:
                services.RegisterConstant<IMagneticStripeReader>(new WndPosMagneticStripeReader());
                break;
            case MsrReaderLib.MsrKeyboard:
                services.RegisterConstant<IMagneticStripeReader>(MsrKeyboard.Instance);
                break;
        }
    }
}
