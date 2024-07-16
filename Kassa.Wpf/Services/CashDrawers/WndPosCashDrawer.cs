using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Splat;
using Windows.Devices.PointOfService;

namespace Kassa.Wpf.Services.CashDrawers;
internal sealed class WndPosCashDrawer : ICashDrawer, IEnableLogger
{

    public static async Task<CashDrawer?> FindFirstCashDrawer()
    {
        var cashDrawer = await CashDrawer.GetDefaultAsync();

        LogHost.Default.Debug($"Cash drawer found: {cashDrawer?.DeviceId ?? "not found"}");

        if (cashDrawer == null)
        {
            LogHost.Default.Debug($"Trying find cash drawer with DeviceWatcher");

            cashDrawer = await DeviceHelper.GetFirstCashDrawerAsync();

            LogHost.Default.Debug($"Cash drawer found: {cashDrawer?.DeviceId ?? "not found"}");
        }

        if (cashDrawer == null)
        {
            LogHost.Default.Debug($"Trying find cash drawer with DeviceInformation");

            cashDrawer = await DeviceHelper.GetFirstDeviceAsyncWithDeviceInformation(
                CashDrawer.GetDeviceSelector(),
                async x => await CashDrawer.FromIdAsync(x));

            LogHost.Default.Debug($"Cash drawer found: {cashDrawer?.DeviceId ?? "not found"}");
        }

        if (cashDrawer == null)
        {
            return null;
        }

        return cashDrawer;
    }

    public async Task Open()
    {
        using var cashDrawer = await FindFirstCashDrawer();

        if (cashDrawer == null)
        {
            this.Log().Warn("No cash drawer found");
            return;
        }
        else
        {
            this.Log().Info($"Cash drawer found: {cashDrawer.DeviceId}");
        }

        using var claimedDrawer = await cashDrawer.ClaimDrawerAsync();

        if (claimedDrawer == null)
        {
            this.Log().Warn("Failed to claim cash drawer");
            return;
        }

        await claimedDrawer.OpenDrawerAsync();

    }
}
