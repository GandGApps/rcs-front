﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic;
public static class BuisnessLogicExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNotInitialized<T>(this T service) where T: IInitializableService
    {
        if (!service.IsInitialized)
        {
            throw new InvalidOperationException($"Service {service.GetType().Name} is not initialized");
        }
    }
}