using System.Buffers;
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

        public void Pipes()
        {
            Memory<byte> m = Memory<byte>.Empty;
            //ReadOnlySequence<byte> 
            //Buffer.BlockCopy(
        }

        public void BinarySearch()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var span = new Span<int>(array);

            var position = span.BinarySearch(3);
        }
    }
}
