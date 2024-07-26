using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Kassa.Shared.RcsLocator;

namespace Kassa.Shared.Locator;

public static class RcsLocator
{
    public static readonly RcsLocatorBuilder Builder = new();

    public static readonly RcsScopedLocator Scoped = new();

    public static T GetRequiredService<T>() where T : class
    {
        var descriptor = Builder.FrozenServices[typeof(T)];
        unsafe
        {
            return Unsafe.As<T>(descriptor.Instance ?? descriptor.Creator());
        }
    }

    /// <summary>
    /// You should not call this method directly. It's need for code generation only.
    /// </summary>
    /// <typeparam name="T">
    /// Do not use this parameter. It's need for code generation only.
    /// </typeparam>
    public static void CreateAndInject<T>(out T instance)
    {
        instance = default!;
        ThrowHelper.ThrowNotSupportedException("You should not call this method directly. It's need for code generation only.");
    }
}