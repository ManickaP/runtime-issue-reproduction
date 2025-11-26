using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

var p = new Poo();
await p.PooPoo("Jalovice kakala na poli");

public class Poo
{
    private OutgoingBuffer _ob;
    private MemoryStream _s;

    public Poo()
    {
        _s = new MemoryStream();
        _ob = new OutgoingBuffer(1024, _s);
    }

    public async ValueTask PooPoo(string data)
    {
        _ob = await _ob.WriteBytesAsync(Encoding.UTF8.GetBytes(data), default);
        _ob = await _ob.FlushAsync(default);
        var x = Encoding.UTF8.GetBytes(data);
        x.CopyTo(_ob.Free);
        _ob.Commit(x.Length);
        await _ob.DisposeAsync();
        Console.WriteLine(_s.ToArray().Length);
        Console.WriteLine(Encoding.UTF8.GetString(_s.ToArray()));
    }
}

struct OutgoingBuffer : IAsyncDisposable
{
    private byte[] _buffer;
    //private Memory<byte> _free;
    private int _free;
    private readonly Stream _stream;

    private int _state;

    public OutgoingBuffer(int capacity, Stream stream)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(capacity, 0, nameof(capacity));

        _buffer = ArrayPool<byte>.Shared.Rent(capacity);
        _free = _buffer.Length;
        _stream = stream;
    }

    public int TotalCapacity => _buffer.Length;

    private Memory<byte> GetFreeMemory()
    {
        Debug.Assert(_free >= 0);
        Debug.Assert(_free <= _buffer.Length);

        return new Memory<byte>(_buffer, _buffer.Length - _free, _free);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Memory<byte> Free => Interlocked.CompareExchange(ref _state, 1, 0) switch
    {
        0 => GetFreeMemory(),
        1 => throw new InvalidOperationException($"Concurrent operations on {nameof(OutgoingBuffer)} are not allowed."),
        _ => throw new ObjectDisposedException(nameof(OutgoingBuffer))
    };

    public void Commit(int count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, _free, nameof(count));
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 0, nameof(count));

        try
        {
            _free = Interlocked.CompareExchange(ref _state, 1, 1) switch
            {
                0 => throw new InvalidOperationException($"Unexpected operation on {nameof(OutgoingBuffer)}, state should be active."),
                1 => _free - count,
                _ => throw new ObjectDisposedException(nameof(OutgoingBuffer)),
            };
        }
        finally
        {
            Volatile.Write(ref _state, 0);
        }
    }

    public async ValueTask EnsureFreeAsync(int capacity, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(capacity, _buffer.Length, nameof(capacity));

        switch (Interlocked.CompareExchange(ref _state, 1, 0))
        {
            case 0: break;
            case 1: throw new InvalidOperationException($"Concurrent operations on {nameof(OutgoingBuffer)} are not allowed.");
            default: throw new ObjectDisposedException(nameof(OutgoingBuffer));
        }

        try
        {
            if (_free < capacity)
            {
                await _stream.WriteAsync(_buffer.AsMemory().Slice(0, _buffer.Length - _free), cancellationToken);
                _free = _buffer.Length;
            }
        }
        finally
        {
            Volatile.Write(ref _state, 0);
        }
    }

    public async ValueTask<OutgoingBuffer> WriteBytesAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken)
    {
        switch (Interlocked.CompareExchange(ref _state, 1, 0))
        {
            case 0: break;
            case 1: throw new InvalidOperationException($"Concurrent operations on {nameof(OutgoingBuffer)} are not allowed.");
            default: throw new ObjectDisposedException(nameof(OutgoingBuffer));
        }

        try
        {
            do
            {
                if (_free < source.Length)
                {
                    source.Slice(0, _free).CopyTo(GetFreeMemory());
                    source = source.Slice(_free);
                    await _stream.WriteAsync(_buffer, cancellationToken);
                    _free = _buffer.Length;
                }
                else
                {
                    source.CopyTo(GetFreeMemory());
                    _free -= source.Length;
                    source = Memory<byte>.Empty;
                }
            } while (source.Length > 0);
        }
        finally
        {
            Volatile.Write(ref _state, 0);
        }
        return this;
    }

    public async ValueTask<OutgoingBuffer> FlushAsync(CancellationToken cancellationToken)
    {
        switch (Interlocked.CompareExchange(ref _state, 1, 0))
        {
            case 0: break;
            case 1: throw new InvalidOperationException($"Concurrent operations on {nameof(OutgoingBuffer)} are not allowed.");
            default: throw new ObjectDisposedException(nameof(OutgoingBuffer));
        }

        try
        {
            if (_free < _buffer.Length)
            {
                await _stream.WriteAsync(_buffer.AsMemory(0, _buffer.Length - _free), cancellationToken);
            }
            await _stream.FlushAsync(cancellationToken);
            _free = _buffer.Length;
        }
        finally
        {
            Volatile.Write(ref _state, 0);
        }
        return this;
    }

    public async ValueTask DisposeAsync()
    {
        switch (Interlocked.CompareExchange(ref _state, 2, 0))
        {
            case 0: break;
            case 1: throw new InvalidOperationException($"Concurrent operations on {nameof(OutgoingBuffer)} are not allowed.");
            default: return;
        }

        var buffer = Interlocked.Exchange(ref _buffer, null!);
        Debug.Assert(buffer is not null);

        if (_free < buffer.Length)
        {
            await _stream.WriteAsync(buffer.AsMemory(0, buffer.Length - _free), default);
        }
        await _stream.FlushAsync();

        _free = default;
        ArrayPool<byte>.Shared.Return(buffer);
    }
}