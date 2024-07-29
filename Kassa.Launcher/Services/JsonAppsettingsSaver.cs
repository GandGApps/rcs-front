using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Kassa.Launcher.Services;
internal sealed class JsonAppsettingsSaver : IOptionManager
{
    private const string EnvironmentVariableKassaInstallPath = "KASSA_INSTALL_PATH";
    private const string LocalAppSettingsPath = "appsettings.local.json";

    public T GetOption<T>(string key)
    {
        var filePath = Environment.GetEnvironmentVariable(EnvironmentVariableKassaInstallPath);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException($"The environment variable {EnvironmentVariableKassaInstallPath} is not set.");
        }

        // get or create the appsettings.local.json file
        var appSettingsLocalPath = Path.Combine(filePath, LocalAppSettingsPath);

        if (!File.Exists(appSettingsLocalPath))
        {
            File.WriteAllText(appSettingsLocalPath, "{}");
        }

        var jsonNode = JsonNode.Parse(File.ReadAllText(appSettingsLocalPath));

        if (jsonNode is null)
        {
            throw new InvalidOperationException("The appsettings.local.json file is empty.");
        }

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return jsonNode[key].Deserialize<T>()!;
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }

    public void SaveOption<T>(string key, T value)
    {
        var filePath = Environment.GetEnvironmentVariable(EnvironmentVariableKassaInstallPath);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException($"The environment variable {EnvironmentVariableKassaInstallPath} is not set.");
        }

        // get or create the appsettings.local.json file
        var appSettingsLocalPath = Path.Combine(filePath, LocalAppSettingsPath);

        if (!File.Exists(appSettingsLocalPath))
        {
            File.WriteAllText(appSettingsLocalPath, "{}");
        }

        var jsonNode = JsonNode.Parse(File.ReadAllText(appSettingsLocalPath));

        if (jsonNode is null)
        {
            throw new InvalidOperationException("The appsettings.local.json file is empty.");
        }

        if (value is int int32)
        {
            jsonNode[key] = int32;
        }
        else if (value is string str)
        {
            jsonNode[key] = str;
        }
        else
        {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            jsonNode[key] = JsonSerializer.Serialize(value);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }



        File.WriteAllText(appSettingsLocalPath, jsonNode.ToString());

    }
}
