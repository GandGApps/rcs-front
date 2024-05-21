using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Services;

namespace Kassa.BuisnessLogic;
public static class BuisnessLogicExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Conditional("DEBUG")]
    public static void ThrowIfNotInitialized<T>(this T service) where T : IInitializableService
    {
        if (!service.IsInitialized)
        {
            throw new InvalidOperationException($"Service {service.GetType().Name} is not initialized");
        }
    }

    public static bool IsShiftStarted(this IShiftService shift)
    {
        return shift.CurrentShift.Value != null && shift.CurrentShift.Value.IsStarted.Value;
    }

    public static IObservable<bool> IsShiftStartedObservable(this IShiftService shift)
    {
        return shift.CurrentShift.SelectMany(x => x is null ? Observable.Return<bool?>(null) : x.IsStarted.Select(x => (bool?)x))
            .Select(x => x.HasValue && x.Value);
    }

    
    public static bool IsCashierShiftStarted(this IShiftService cashier)
    {
        return cashier.CurrentCashierShift.Value != null && cashier.CurrentCashierShift.Value.IsStarted.Value;
    }

    public static IObservable<bool> IsCashierShiftStartedObservable(this IShiftService cashier)
    {
        return cashier.CurrentCashierShift.SelectMany(x => x is null ? Observable.Return<bool?>(null) : x.IsStarted.Select(x => (bool?)x))
                    .Select(x => x.HasValue && x.Value);
    }

    public static string GuidToPrettyString(this Guid guid)
    {
        return guid.ToString("N")[..5];
    }

    public static int GuidToPrettyInt(this Guid guid)
    {
        return int.Parse(guid.ToString("N")[..5], System.Globalization.NumberStyles.HexNumber) % 10000;
    }

    public static string GuidToPrettyString(this Guid? guid)
    {
        return guid is null ? "???" : guid.Value.ToString("N")[..5];
    }

}
