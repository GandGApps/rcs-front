using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Kassa.Wpf.Services.MagneticStripeReaders;
internal static class MscReaderLibServices
{
    public static void RegisterMsrReaderPosLib(IConfiguration configuration)
    {
        var msrReaderLibString = configuration.GetValue<string>(nameof(MsrReaderLib));
        var msrReaderLib = Enum.TryParse<MsrReaderLib>(msrReaderLibString, true, out var pos) ? pos : MsrReaderLib.MsrPos;

        switch (msrReaderLib)
        {
            case MsrReaderLib.MsrPos:
                var msrPos = new WndPosMagneticStripeReader();
                Dispatcher.CurrentDispatcher.InvokeAsync(async () => await msrPos.TryClaim());
                LogHost.Default.Info("MsrPos reader is used");
                RcsLocatorBuilder.AddSingleton<IMagneticStripeReader>(msrPos);
                break;
            case MsrReaderLib.MsrKeyboard:
                LogHost.Default.Info("MsrKeyboard reader is used");
                RcsLocatorBuilder.AddSingleton<IMagneticStripeReader>(MsrKeyboard.Instance);
                break;
        }
    }
}
