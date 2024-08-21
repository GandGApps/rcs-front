using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.Shared.Logging;
public sealed class MultiLogger : ICollection<ILogger>, ILogger
{

    private readonly List<ILogger> _loggers = [];

    public MultiLogger(ILogger[] loggers)
    {
        _loggers.AddRange(loggers);
    }

    public int Count => ((ICollection<ILogger>)_loggers).Count;

    public bool IsReadOnly => ((ICollection<ILogger>)_loggers).IsReadOnly;

    LogLevel ILogger.Level => 0;

    public void Add(ILogger item) => ((ICollection<ILogger>)_loggers).Add(item);

    public MultiLogger AddLogger(ILogger logger)
    {
        _loggers.Add(logger);

        return this;
    }

    public void Clear() => ((ICollection<ILogger>)_loggers).Clear();
    public bool Contains(ILogger item) => ((ICollection<ILogger>)_loggers).Contains(item);
    public void CopyTo(ILogger[] array, int arrayIndex) => ((ICollection<ILogger>)_loggers).CopyTo(array, arrayIndex);
    public IEnumerator<ILogger> GetEnumerator() => ((IEnumerable<ILogger>)_loggers).GetEnumerator();
    public bool Remove(ILogger item) => ((ICollection<ILogger>)_loggers).Remove(item);
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_loggers).GetEnumerator();

    void ILogger.Write(string message, LogLevel logLevel)
    {
        foreach (var logger in _loggers)
        {
            logger.Write(message, logLevel);
        }
    }

    void ILogger.Write(Exception exception, string message, LogLevel logLevel)
    {
        foreach (var logger in _loggers)
        {
            logger.Write(exception, message, logLevel);
        }
    }

    void ILogger.Write(string message, Type type, LogLevel logLevel)
    {
        foreach (var logger in _loggers)
        {
            logger.Write(message, type, logLevel);
        }
    }

    void ILogger.Write(Exception exception, string message, Type type, LogLevel logLevel)
    {
        foreach (var logger in _loggers)
        {
            logger.Write(exception, message, type, logLevel);
        }
    }
}
