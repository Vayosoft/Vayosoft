using System.Buffers;
using System.Text;

namespace Vayosoft.UnitTests.Pipelines
{
    public interface IBytesProcessor
    {
        Task ProcessBytesAsync(ReadOnlySequence<byte> bytesSequence, CancellationToken token);
    }

    public class ConsoleBytesProcessor : IBytesProcessor, IDisposable
    {
        private readonly FileStream _fileStream = new FileStream("buffer", FileMode.Create);

        public Task ProcessBytesAsync(ReadOnlySequence<byte> bytesSequence, CancellationToken token)
        {
            if (bytesSequence.IsSingleSegment)
            {
                ProcessSingle(bytesSequence.First.Span);
            }
            else
            {
                foreach (var segment in bytesSequence)
                {
                    ProcessSingle(segment.Span);
                }
            }

            return Task.CompletedTask;
        }

        private void ProcessSingle(ReadOnlySpan<byte> span)
        {
            _fileStream.Write(span);
            _fileStream.Write(Encoding.UTF8.GetBytes("...\r\n"));
            _fileStream.Flush();
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
        }
    }

}
