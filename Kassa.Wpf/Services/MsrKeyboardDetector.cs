using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Wpf.Services;

/// <summary>
/// Helper class for <see cref="MsrKeyboard"/> to detect keyboard input.
/// </summary>
internal sealed class MsrKeyboardDetector
{
    private readonly StringBuilder _buffer = new();
    private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);


    public bool IsMsr(char ы,[NotNullWhen(true)] out string? data)
    {

        data = null;
        return false;
    }
}
