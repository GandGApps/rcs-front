using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.Shared.Logging;
public record struct LogInfo(LogLevel LogLevel, string Message, Type? Type, Exception? Exception)
{
    public static implicit operator (LogLevel logLevel, string message, Type? type, Exception? exception)(LogInfo value)
    {
        return (value.LogLevel, value.Message, value.Type, value.Exception);
    }

    public static implicit operator LogInfo((LogLevel logLevel, string message, Type? type, Exception? exception) value)
    {
        return new LogInfo(value.logLevel, value.message, value.type, value.exception);
    }
}