// This code is ported and modified from Magnetic stripe reader sample (microsoft/Windows -universal-samples)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;

namespace Kassa.Wpf.Services;
internal static class DeviceHelper
{
    // We use a DeviceWatcher instead of DeviceInformation.FindAllAsync because
    // the DeviceWatcher will let us see the devices as they are discovered,
    // whereas FindAllAsync returns results only after discovery is complete.

    public static async Task<T?> GetFirstDeviceAsync<T>(string selector, Func<string, Task<T>> convertAsync)
        where T : class
    {
        var completionSource = new TaskCompletionSource<T?>();
        var pendingTasks = new List<Task>();
        var watcher = DeviceInformation.CreateWatcher(selector);

        watcher.Added += (DeviceWatcher sender, DeviceInformation device) =>
        {
            async Task lambda(string id)
            {
                var t = await convertAsync(id);
                if (t != null)
                {
                    completionSource.TrySetResult(t);
                }
            }
            pendingTasks.Add(lambda(device.Id));
        };

        watcher.EnumerationCompleted += async (DeviceWatcher sender, object args) =>
        {
            // Wait for completion of all the tasks we created in our "Added" event handler.
            await Task.WhenAll(pendingTasks);
            // This sets the result to "null" if no task was able to produce a device.
            completionSource.TrySetResult(null);
        };

        watcher.Removed += (DeviceWatcher sender, DeviceInformationUpdate args) =>
        {
            // We don't do anything here, but this event needs to be handled to enable realtime updates.
            // See https://aka.ms/devicewatcher_added.
        };

        watcher.Updated += (DeviceWatcher sender, DeviceInformationUpdate args) =>
        {
            // We don't do anything here, but this event needs to be handled to enable realtime updates.
            // See https://aka.ms/devicewatcher_added.
        };

        watcher.Start();

        // Wait for enumeration to complete or for a device to be found.
        var result = await completionSource.Task;

        watcher.Stop();

        return result;
    }

    public static Task<MagneticStripeReader?> GetFirstMagneticStripeReaderAsync(PosConnectionTypes connectionTypes = PosConnectionTypes.All) => GetFirstDeviceAsync(MagneticStripeReader.GetDeviceSelector(connectionTypes), async (id) => await MagneticStripeReader.FromIdAsync(id));
}
