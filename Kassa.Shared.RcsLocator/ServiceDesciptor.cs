using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

namespace Kassa.Shared.Locator;
internal sealed unsafe class ServiceDesciptor(Type serviceType, delegate*managed<object?> factory)
{
    public Type ServiceType => serviceType;
    public delegate* managed<object?> Factory => factory;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? GetService() => Factory();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetService<T>() where T : class => Unsafe.As<T>(Factory());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetRequiredService<T>() where T : class
    {
        var service = Unsafe.As<T>(Factory());

        if (service is null)
        {
            ThrowHelper.ThrowInvalidOperationException($"Service of type {ServiceType} is null.");
        }

        return service;
    }
}