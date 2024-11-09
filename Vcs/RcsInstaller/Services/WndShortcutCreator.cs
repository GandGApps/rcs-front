using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TruePath;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RcsInstaller.Services;
public sealed class WndShortcutCreator : IShortcutCreator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WndShortcutCreator> _logger;

    public WndShortcutCreator(IConfiguration configuration, ILogger<WndShortcutCreator> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task CreateSrotcut(AbsolutePath path, string name)
    {
        var executablePath = _configuration["RcsBinName"];
        

        _logger.LogInformation("Creating shortcut for {name} at {path}", name, path);
        _logger.LogInformation("Executable path: {executablePath}", executablePath);

        if (string.IsNullOrWhiteSpace(executablePath) || !System.IO.File.Exists((path / executablePath).Value)) 
        {
            ThrowHelper.ThrowArgumentException(nameof(executablePath), "The executable path provided in the configuration (RcsBinName) is invalid or missing. It must be a valid path to an existing executable file.");
        }

        var targetPath = (path / executablePath).Value;
        _logger.LogInformation("Target path is {target path}", targetPath);

        var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{name}.lnk");

        _logger.LogInformation("Shortcut path: {shortcutPath}", shortcutPath);

        var directoryPath = Directory.Exists(path.Value) ? path.Value : Path.GetDirectoryName(targetPath);

        _logger.LogInformation("Directory path: {directoryPath}", directoryPath);

        if (directoryPath is null)
        {
            ThrowHelper.ThrowArgumentException(nameof(path), $"The provided path '{path.Value}' is invalid. It must be a valid directory path or lead to an existing file.");
        }

        var shell = new WshShell();

        _logger.LogInformation("WshShell created");

        var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

        _logger.LogInformation("IWshShortcut created");

        var iconPath = Path.Combine(directoryPath, "Logo.ico");
        _logger.LogInformation("Icon path: {iconPath}", iconPath);

        shortcut.IconLocation = iconPath;

        _logger.LogInformation("Icon location set to {iconPath}", iconPath);

        shortcut.TargetPath = targetPath;

        _logger.LogInformation("Shortcut properties set");

        shortcut.Save();

        _logger.LogInformation("Shortcut created at {shortcutPath}", shortcutPath);

        return Task.CompletedTask;
    }
}
