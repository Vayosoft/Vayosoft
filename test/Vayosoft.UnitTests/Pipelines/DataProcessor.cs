using System.Buffers;
using System.IO.Pipelines;

namespace Vayosoft.UnitTests.Pipelines
{
    public class DataProcessor
    {
        private readonly IBytesProcessor _bytesProcessor;
        private readonly PipeReader _pipeReader;

        public DataProcessor(IBytesProcessor bytesProcessor, PipeReader pipeReader)
        {
            _bytesProcessor = bytesProcessor ?? throw new ArgumentNullException(nameof(bytesProcessor));
            _pipeReader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
        }

        public async Task StartProcessingDataAsync(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                ReadResult result = await _pipeReader.ReadAsync(token);
                ReadOnlySequence<byte> buffer = result.Buffer;

                await _bytesProcessor.ProcessBytesAsync(buffer, token);

                _pipeReader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            await _pipeReader.CompleteAsync();
        }
    }
}
