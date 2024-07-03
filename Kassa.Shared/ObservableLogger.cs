using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using Splat;

namespace Kassa.Shared;
public sealed class ObservableLogger : ILogger, IObservable<LogInfo>, Serilog.ILogger
{
    private static readonly Subject<LogInfo> _subject = new();

    public static IObservable<LogInfo> LogStream => _subject;

    public LogLevel Level
    {
        get;
    }

    public IDisposable Subscribe(IObserver<LogInfo> observer) => _subject.Subscribe(observer);
    public void Write([Localizable(false)] string message, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, null, null);
        _subject.OnNext(logInfo);
    }

    public void Write(Exception exception, [Localizable(false)] string message, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, null, exception);
        _subject.OnNext(logInfo);
    }

    public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, type, null);
        _subject.OnNext(logInfo);
    }

    public void Write(Exception exception, [Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, type, exception);
        _subject.OnNext(logInfo);
    }

    public void Write(LogEvent logEvent)
    {
        var logInfo = new LogInfo(ToLogLever(logEvent.Level), logEvent.RenderMessage(), null, logEvent.Exception);
        _subject.OnNext(logInfo);
    }

    private static LogLevel ToLogLever(LogEventLevel logEvent) => logEvent switch
    {
        LogEventLevel.Verbose => LogLevel.Debug,
        LogEventLevel.Debug => LogLevel.Debug,
        LogEventLevel.Information => LogLevel.Info,
        LogEventLevel.Warning => LogLevel.Warn,
        LogEventLevel.Error => LogLevel.Error,
        LogEventLevel.Fatal => LogLevel.Fatal,
        _ => LogLevel.Debug
    };
    
}

