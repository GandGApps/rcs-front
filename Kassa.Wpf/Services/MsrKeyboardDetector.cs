using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using ReactiveUI;
using System.Timers;
using Splat;
using System.Windows.Threading;

namespace Kassa.Wpf.Services;

/// <summary>
/// Helper class for <see cref="MsrKeyboard"/> to detect keyboard input.
/// </summary>
internal sealed class MsrKeyboardDetector : ReactiveObject, IEnableLogger
{
    private readonly StringBuilder _buffer = new();
    private readonly System.Timers.Timer _timer;
    private const int TimeoutMs = 180;

    public MsrKeyboardDetector()
    {
        _timer = new(TimeoutMs);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = false;
    }

    public void TryDetect(string? text)
    {
        if (text == null)
        {
            return;
        }

        this.Log().Debug("TryDetect called with text: {text}", text);

        _timer.Stop();
        _buffer.Append(text);

        this.Log().Debug("Current buffer content: {buffer}", _buffer.ToString());

        _timer.Start();

        this.Log().Debug("Timer restarted");
    }

    private void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        this.Log().Debug("Timer elapsed");

        var data = _buffer.ToString();
        if (!string.IsNullOrEmpty(data))
        {
            this.Log().Debug("Buffer has data, calling OnMsrCardData");
            Dispatcher.CurrentDispatcher.Invoke(() => MsrKeyboard.Instance.OnMsrCardData(data));
            _buffer.Clear();
        }
        else
        {
            this.Log().Debug("Buffer is empty, not calling OnMsrCardData");
        }
    }
}