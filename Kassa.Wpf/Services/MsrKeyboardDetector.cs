using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.Wpf.Services;

/// <summary>
/// Helper class for <see cref="MsrKeyboard"/> to detect keyboard input.
/// </summary>
internal sealed class MsrKeyboardDetector: IEnableLogger
{
    private readonly StringBuilder _buffer = new();
    private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);
    private DateTime _lastKeystroke = DateTime.Now;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="data">Final data from Msr</param>
    /// <returns></returns>
    public bool IsMsr(string? text, [NotNullWhen(true)] out string? data)
    {
        var elapsed = DateTime.Now - _lastKeystroke;
        if (elapsed > _timeout)
        {
            this.Log().Info("Msr timeout. Clear buffer");
            _buffer.Clear();
        }

        _lastKeystroke = DateTime.Now;

        if (text == " " && _buffer.Length > 0)
        {
            this.Log().Info("Msr data: {0}", _buffer);
            data = _buffer.ToString();
            _buffer.Clear();
            return true;
        }

        _buffer.Append(text);

        
        data = null;
        return false;
    }
}
