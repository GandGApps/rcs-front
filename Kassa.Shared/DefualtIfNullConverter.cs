using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kassa.Shared;
internal sealed class DefaultIfNullConverter<T>: JsonConverter<T> where T : struct
{
    private static readonly JsonConverter<T> defaultConverter =
        (JsonConverter<T>)JsonSerializerOptions.Default.GetConverter(typeof(T));

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        return defaultConverter.Read(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        defaultConverter.Write(writer, value, options);
    }
}
