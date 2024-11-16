using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RcsInstaller.Configurations;
using RcsInstaller.Progress;

namespace RcsInstaller.Services;
internal sealed class SimpleRemover: IRemover
{
    private readonly IAppRegistry _appRegistry;
    private readonly ILogger<SimpleRemover> _logger;

    public SimpleRemover(IAppRegistry appRegistry, ILogger<SimpleRemover> logger)
    {
        _appRegistry = appRegistry;
        _logger = logger;
    }

    public async Task RemoveAsync(Action<ProgressState> value)
    {
        var basePath = await _appRegistry.GetBasePath() ?? throw new InvalidOperationException("The application registry does not contain information about the base path. Ensure that the application is correctly registered with a valid base path before attempting this operation.");

        var files = Directory.EnumerateFiles(basePath.Value, "*", SearchOption.AllDirectories).ToImmutableArray();

        for(var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var progress = Math.Clamp(0, (double)i / files.Length, 0.99);
            value(new ProgressState($"Удаление \"{file}\"...", progress));

            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file \"{file}\"", file);
            }
        }

        try
        {
            await _appRegistry.UnRegister();
        }
        catch
        {
            _logger.LogError("Failed to delete the registry entry");
        }
    }
}
