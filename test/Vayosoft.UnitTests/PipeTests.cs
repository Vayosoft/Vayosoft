using System.IO.Pipelines;
using System.IO.Pipes;
using System.Text;
using Vayosoft.UnitTests.Pipelines;
using Xunit.Abstractions;

namespace Vayosoft.UnitTests
{
    public class PipeTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public PipeTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task Main() 
        {
            var pipe = new Pipe();
            var dataWriter = new PipeDataWriter(pipe.Writer, "testPipe");
            var dataProcessor = new DataProcessor(new ConsoleBytesProcessor(), pipe.Reader);

            var cts = new CancellationTokenSource();
            var namedServer = Task.Run(async () =>
            {
                var server = new NamedPipeServerStream("testPipe", PipeDirection.Out);
                
                await server.WaitForConnectionAsync(CancellationToken.None);
                foreach (var i in Enumerable.Range(0,100))
                {
                    await server.WriteAsync(Encoding.UTF8.GetBytes($"line_{i}\r\n"), CancellationToken.None);
                }

                await Task.Delay(1000, CancellationToken.None);
                cts.Cancel();
                server.Close();
            }, CancellationToken.None);

         
            await Task.WhenAll(dataWriter.ReadFromPipeAsync(cts.Token), dataProcessor.StartProcessingDataAsync(cts.Token),
                namedServer);
        }
    }
}
