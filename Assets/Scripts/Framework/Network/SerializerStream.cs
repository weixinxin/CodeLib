using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class SerializerStream : MemoryStream
{
    public SerializerStream(int capacity) : base(capacity)
    {

    }

    public override bool CanSeek { get => false; }

    public override long Position
    {
        get
        {
            return base.Position;
        }
        set => throw new NotSupportedException();
    }
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
    public override long Seek(long offset, SeekOrigin loc)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public void SetPosition(long value)
    {
        base.Position = value;
    }

    public void SafeSetLength(long value)
    {
        base.SetLength(value);
    }
}