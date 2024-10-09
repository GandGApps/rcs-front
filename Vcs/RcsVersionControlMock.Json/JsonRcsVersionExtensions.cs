using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TruePath;

namespace RcsVersionControlMock.Json;
public static class JsonRcsVersionExtensions
{

    public static IServiceCollection AddJsonRcsVersionControl(this IServiceCollection services)
    {
        services.AddSingleton<IRcsVersionControlFactory, JsonRcsVersionFactory>();
        return services;
    }

    public static IServiceCollection AddJsonRcsVersionControl(this IServiceCollection services, string path)
    {
        services.AddSingleton<IRcsVersionControl>(sp =>
        {
            var rcs = JsonRcsVersionFactory.CreateRcsVC(path, sp);

            return rcs;
        });

        return services;
    }

    internal sealed class JsonRcsVersionFactory(IServiceProvider serviceProvider) : IRcsVersionControlFactory
    {


        public IRcsVersionControl Create(string appPath) => CreateRcsVC(appPath, serviceProvider);

        public static IRcsVersionControl CreateRcsVC(string appPath, IServiceProvider serviceProvider)
        {
            var appPathAbsolutePath = new AbsolutePath(appPath);
            var baseDdrectory = appPathAbsolutePath.Parent ?? throw new NotSupportedException();

            var rcsPath = baseDdrectory / ".rcs";
            var versionStore = new JsonVersionStore(rcsPath, serviceProvider.GetRequiredService<ILogger<JsonVersionStore>>());
            var changesStore = new JsonVersionChangesStore(rcsPath, versionStore, serviceProvider.GetRequiredService<ILogger<JsonVersionChangesStore>>());

            return new RcsVersionControl(changesStore, versionStore, serviceProvider.GetRequiredService<ILogger<RcsVersionControl>>(), appPathAbsolutePath);
        }
    }

}