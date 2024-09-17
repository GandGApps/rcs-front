﻿#if false

namespace Kassa.Shared.Locator
{
    internal static partial class RcsLocatorBuilder
    {
        public static void AddTransient<TService, TImplementation>() where TService : class where TImplementation : TService
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a marker
        }

        public static void AddScoped<TService, TImplementation>() where TService : class, IInitializable where TImplementation : TService
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a marker
        }

        public static void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : TService
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a marker
        }

        public static void AddTransient<TService>()
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a marker
        }

        public static void AddScoped<TService>() where TService : class, IInitializable
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a 
        }

        public static void AddSingleton<TService>()
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a marker
        }

        public static void AddSingleton<TService>(Func<TService> serviceCreator)
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a marker
        }

        public static void AddSingleton<TService, TImplementation>(Func<TImplementation> serviceCreator)
        {
            // This method is needed by the source generator
            // The body of this method is empty because this method is only a marker
        }
    }
}
#endif