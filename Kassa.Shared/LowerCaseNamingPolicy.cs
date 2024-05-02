using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kassa.Shared;
public sealed class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public static readonly LowerCaseNamingPolicy Instance = new();

    public override sealed string ConvertName(string name)
    {
        return string.IsNullOrEmpty(name) || !char.IsUpper(name[0]) ? name : name.ToLower();
    }
}
