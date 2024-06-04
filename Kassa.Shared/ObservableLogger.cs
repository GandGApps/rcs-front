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
    private readonly Subject<LogInfo> subject = new();

    public LogLevel Level
    {
        get;
    }

    public IDisposable Subscribe(IObserver<LogInfo> observer) => subject.Subscribe(observer);
    public void Write([Localizable(false)] string message, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, null, null);
        subject.OnNext(logInfo);
    }

    public void Write(Exception exception, [Localizable(false)] string message, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, null, exception);
        subject.OnNext(logInfo);
    }

    public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, type, null);
        subject.OnNext(logInfo);
    }

    public void Write(Exception exception, [Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
    {
        var logInfo = new LogInfo(logLevel, message, type, exception);
        subject.OnNext(logInfo);
    }

    public void Write(LogEvent logEvent)
    {
        var logInfo = new LogInfo(ToLogLever(logEvent.Level), logEvent.RenderMessage(), null, logEvent.Exception);
        subject.OnNext(logInfo);
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

