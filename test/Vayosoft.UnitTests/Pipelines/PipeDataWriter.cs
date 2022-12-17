using System.IO.Pipelines;
using System.IO.Pipes;

namespace Vayosoft.UnitTests.Pipelines
{
    public class PipeDataWriter
    {
        private readonly NamedPipeClientStream _namedPipe;
        private readonly PipeWriter _pipeWriter;
        private const string ServerName = ".";

        public PipeDataWriter(PipeWriter pipeWriter, string pipeName)
        {
            _pipeWriter = pipeWriter ?? throw new ArgumentNullException(nameof(pipeWriter));
            _namedPipe = new NamedPipeClientStream(ServerName, pipeName, PipeDirection.In);
        }

        public async Task ReadFromPipeAsync(CancellationToken token)
        {
            await _namedPipe.ConnectAsync(token);

            while (true)
            {
                token.ThrowIfCancellationRequested();

                //// при работе с асинхронным методом используем Memory<T>
                //Memory<byte> buffer = _pipeWriter.GetMemory();
                //// асинхронное чтение из именованного канала в Memory<T>
                //// здесь может быть любая операция для получения данных - от считывания с файла до рандомной генерации.
                //int readBytes = await _namedPipe.ReadAsync(buffer, token); 

                // синхронное чтение из именованного канала в запрошенный у PipeWriter Span
                // здесь может быть любая операция для получения данных - от считывания с файла до рандомной генерации.
                int readBytes = _namedPipe.Read(_pipeWriter.GetSpan());

                // если в канале ничего не было, отпускаем поток на полсекунды и пытаемся снова
                // в других случаях при данной ситуации можно выходить
                if (readBytes == 0)
                {
                    await Task.Delay(500, token);
                    continue;
                }

                // указываем, сколько байт мы взяли из канала
                _pipeWriter.Advance(readBytes);

                // флашим данные, чтобы они стали доступны для PipeReader
                FlushResult result = await _pipeWriter.FlushAsync(token);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            await _pipeWriter.CompleteAsync();
        }
    }
}
