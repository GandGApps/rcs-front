using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared.RcsLocator;
internal sealed class ServiceDesciptor(Type serviceType, Func<object>? creator, object? Instance)
{
    public Type ServiceType => serviceType;

    public Func<object>? Creator => creator;

    public object? Instance
    {
        get; set;
    } = Instance;

    public T GetRequiredService<T>() where T : class => Unsafe.As<T>(Instance ?? Creator!());

    public T? GetService<T>() where T : class => Unsafe.As<T?>(Instance ?? Creator!());

    public void CreateInstance()
    {
        if (Instance is not null)
        {
            return;
        }

        Instance = Creator();
    }

    public object? GetService() => Instance ?? Creator();
}

