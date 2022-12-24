using System.Buffers;
using System.Buffers.Text;
using System.IO.Pipelines;

namespace Benchmarks
{
    public class Memory
    {
        public void Extensions()
        {
            MemoryExtensions.IsWhiteSpace("");
            MemoryExtensions.ToLowerInvariant("", Span<char>.Empty);
        }

     
        public void BinarySearch()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var span = new Span<int>(array);

            var position = span.BinarySearch(3);
        }

        private static bool TryParseHeaderLength(ref ReadOnlySequence<byte> buffer, out int length)
        {
            var reader = new SequenceReader<byte>(buffer);
            return reader.TryReadBigEndian(out length);
        }

        private static SequencePosition? FindIndexOf(in ReadOnlySequence<byte> buffer, byte data)
        {
            var reader = new SequenceReader<byte>(buffer);

            while (!reader.End)
            {
                // Search for the byte in the current span.
                var index = reader.CurrentSpan.IndexOf(data);
                if (index != -1)
                {
                    // It was found, so advance to the position.
                    reader.Advance(index);

                    return reader.Position;
                }
                // Skip the current segment since there's nothing in it.
                reader.Advance(reader.CurrentSpan.Length);
            }

            return null;
        }

        private static ReadOnlySpan<byte> NewLine => "\r\n"u8;

        private static bool TryParseLine(ref ReadOnlySequence<byte> buffer,
            out ReadOnlySequence<byte> line)
        {
            var reader = new SequenceReader<byte>(buffer);

            if (reader.TryReadTo(out line, NewLine))
            {
                buffer = buffer.Slice(reader.Position);

                return true;
            }

            line = default;
            return false;
        }

        public static bool TryParse(ReadOnlySequence<byte> source, out int value, out int byteConsumed)
        {
            if (source.IsSingleSegment)
            {
                return Utf8Parser.TryParse(source.FirstSpan, out value, out byteConsumed);
            }
            else
            {
                Span<byte> span = stackalloc byte[128];
                source.CopyTo(span);

                return Utf8Parser.TryParse(span, out value, out byteConsumed);
            }
        }
    }
}
