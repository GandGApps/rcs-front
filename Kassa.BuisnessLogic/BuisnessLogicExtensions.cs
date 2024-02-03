using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public static void ThrowIfNotInitialized<T>(this T service) where T: IInitializableService
    {
        if (!service.IsInitialized)
        {
            throw new InvalidOperationException($"Service {service.GetType().Name} is not initialized");
        }
    }

    public static IObservable<IChangeSet<T,TKey>> TransformAndBind<T,TSource,TKey>(this IObservable<IChangeSet<TSource,TKey>> changes, Func<TSource,T> create) 
        where T: class, IReactiveToChangeSet<TKey, TSource> 
        where TKey: notnull 
        where TSource: notnull
    {
        return changes.TransformWithInlineUpdate(create, (transformed, source) =>
        {
            transformed.Source = source;
        });
    }
}
