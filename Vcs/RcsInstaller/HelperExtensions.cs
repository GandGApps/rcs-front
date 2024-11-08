using Microsoft.Extensions.Configuration;
using Refit;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller;
public static class HelperExtensions
{

    public static readonly Version EmptyVersion = new(0, 0);

    /*
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver dependencyResolver) where T : class
    {
        return dependencyResolver.GetService<T>() ?? throw new NullReferenceException($"Service {typeof(T)}");
    }
    */

    public static IConfigurationBuilder AddEmbeddedJsonFile(this IConfigurationBuilder configuration, string resourceName)
    {
        var host = Assembly.GetEntryAssembly();
        if (host == null)
            return configuration;

        var fullFileName = $"{host.GetName().Name}.{resourceName}";
        using var input = host.GetManifestResourceStream(fullFileName);
        

        if (input != null)
        {
            var copyStream = new MemoryStream();
            input.CopyTo(copyStream);

            copyStream.Seek(0, SeekOrigin.Begin);

            configuration.AddJsonStream(copyStream);
        }

        return configuration;
    }
}
