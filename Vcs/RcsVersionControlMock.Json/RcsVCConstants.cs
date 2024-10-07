using System.Text.Json;

namespace RcsVersionControlMock.Json;

public static class RcsVCConstants
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        TypeInfoResolverChain =
        {
            RcsJsonContext.Default
        }
    };

    public static readonly Version EmptyVersion = new(0, 0);

    public static readonly JsonSerializerOptions WriteIndentededJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        WriteIndented = true,
        TypeInfoResolverChain =
        {
            RcsJsonContext.Default
        }
    };
}
