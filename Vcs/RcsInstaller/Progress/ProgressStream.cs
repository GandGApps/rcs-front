using ExCSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Progress;
public sealed class ProgressStream(Stream input, long? length) : Stream
{
    private readonly long _length = length ?? input.Length;
    private long _position = 0L;

    public override bool CanRead => input.CanRead;

    public override bool CanSeek => input.CanSeek;

    public override bool CanWrite => input.CanWrite;

    public override long Length => input.Length;

    public override long Position
    {
        get => input.Position; set => input.Position = value;
    }

    public event EventHandler<ProgressEventArgs>? UpdateProgress;

    public override void Flush()
    {
        input.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var n = input.Read(buffer, offset, count);
        _position += n;
        UpdateProgress?.Invoke(this, new ProgressEventArgs(1.0f * _position / _length));
        return n;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return input.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        input.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        input.Write(buffer, offset, count);
    }
}
