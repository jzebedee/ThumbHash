using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ThumbHash;

internal readonly ref struct SpanOwner<T>
{
    private readonly T[] _buffer;
    private readonly int _length;

    public static SpanOwner<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(0);
    }

    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ref T r0 = ref MemoryMarshal.GetArrayDataReference(_buffer);
            return MemoryMarshal.CreateSpan(ref r0, _length);
        }
    }

    public SpanOwner<T> WithLength(int length) => new(length, _buffer);

    public SpanOwner(int length) : this(length, ArrayPool<T>.Shared.Rent(length))
    {
    }
    private SpanOwner(int length, T[] buffer)
    {
        _length = length;
        _buffer = buffer;
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_buffer);
    }
}
