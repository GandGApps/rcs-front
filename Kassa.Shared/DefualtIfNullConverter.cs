using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization.Metadata;
using System.Runtime.CompilerServices;

namespace Kassa.Shared;
internal sealed class DefaultIfNullConverter<T>: JsonConverter<T> where T : struct
{

    private static readonly JsonConverter<T> defaultConverter = GetConverter();

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

    private static JsonConverter<T> GetConverter()
    {
        static JsonConverter<T> CastConverter<U>(JsonConverter<U> jsonConverter)
        {
            return (JsonConverter<T>)(JsonConverter)jsonConverter;
        }

        if (typeof(T) == typeof(int))
        {
            return CastConverter(JsonMetadataServices.Int32Converter);
        }

        if(typeof(T) == typeof(bool))
        {
            return CastConverter(JsonMetadataServices.BooleanConverter);
        }

        if(typeof(T) == typeof(double))
        {
            return CastConverter(JsonMetadataServices.DoubleConverter);
        }

        if(typeof(T) == typeof(float))
        {
            return CastConverter(JsonMetadataServices.SingleConverter);
        }

        if(typeof(T) == typeof(byte))
        {
            return CastConverter(JsonMetadataServices.ByteConverter);
        }

        if(typeof(T) == typeof(string))
        {
            return CastConverter(JsonMetadataServices.StringConverter);
        }

        if(typeof(T) == typeof(TimeSpan))
        {
            return CastConverter(JsonMetadataServices.TimeSpanConverter);
        }

        if(typeof(T) == typeof(DateTime))
        {
            return CastConverter(JsonMetadataServices.DateTimeConverter);
        }

        if(typeof(T) == typeof(Guid))
        {
            return CastConverter(JsonMetadataServices.GuidConverter);
        }

        throw new NotSupportedException();
    }
}

[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(Guid))]
internal sealed partial class DefaultContext: JsonSerializerContext
{

    internal static readonly JsonSerializerOptions NewOptions = new()
    {
        TypeInfoResolver = Default
    };
}