using BenchmarkDotNet.Attributes;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;

namespace Benchmarks
{
    //[DisassemblyDiagnoser(printSource: true)]
    //[MemoryDiagnoser]
    //public class ReadLinesChannels
    //{
    //    private static ReadOnlySpan<byte> NewLine => "\r\n"u8;
    //    private Channel<MyData> _channel;
    //    private int _count;

    //    [Benchmark(Baseline = true)]
    //    public async Task ProcessTasksAsync()
    //    {
    //        while (true)
    //        {
    //            ReadResult result = await _reader.ReadAsync().ConfigureAwait(false);
    //            ReadOnlySequence<byte> buffer = result.Buffer;

    //            AddToTaskList(ref buffer);

    //            _reader.AdvanceTo(buffer.Start, buffer.End);

    //            if (result.IsCompleted) break;
    //        }
    //        await Task.WhenAll(_tasks).ConfigureAwait(false);

    //        await _reader.CompleteAsync().ConfigureAwait(false);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    private void AddToTaskList(ref ReadOnlySequence<byte> buffer)
    //    {
    //        string str = null;

    //        if (buffer.IsSingleSegment)
    //        {
    //            var span = buffer.FirstSpan;
    //            int consumed;
    //            while (span.Length > 0)
    //            {
    //                var newLine = span.IndexOf(NewLine);

    //                if (newLine == -1) break;

    //                var line = span.Slice(0, newLine);
    //                str = Encoding.UTF8.GetString(line);

    //                // add to Task list
    //                _tasks.Add(ProcessLineCoreAsync(new MyData { Content = str, No = ++_counter }));

    //                consumed = line.Length + NewLine.Length;
    //                span = span.Slice(consumed);
    //                buffer = buffer.Slice(consumed);
    //            }
    //        }
    //        else
    //        {
    //            var sequenceReader = new SequenceReader<byte>(buffer);

    //            while (!sequenceReader.End)
    //            {
    //                while (sequenceReader.TryReadTo(out ReadOnlySequence<byte> line, NewLine))
    //                {
    //                    str = Encoding.UTF8.GetString(line);

    //                    // add to Task list
    //                    _tasks.Add(ProcessLineCoreAsync(new MyData { Content = str, No = ++_counter }));
    //                }

    //                buffer = buffer.Slice(sequenceReader.Position);
    //                sequenceReader.Advance(buffer.Length);
    //            }
    //        }
    //    }

    //    [Benchmark]
    //    public async Task ProcessTasksUsingChannelAsync()
    //    {
    //        while (true)
    //        {
    //            ReadResult result = await _reader.ReadAsync().ConfigureAwait(false);
    //            ReadOnlySequence<byte> buffer = result.Buffer;

    //            WriteToChannel(ref buffer);

    //            _reader.AdvanceTo(buffer.Start, buffer.End);

    //            if (result.IsCompleted) break;
    //        }

    //        // mark the channel as being complete, meaning no more items will be written to it.
    //        _channel.Writer.TryComplete();

    //        // await the Task that completes when no more data will ever be available to be read from this channel.
    //        await _channel.Reader.Completion.ConfigureAwait(false);

    //        // wait the ProcessLineCoreAsync to finish
    //        await Task.WhenAll(_channelReaderTasks).ConfigureAwait(false);

    //        await _reader.CompleteAsync().ConfigureAwait(false);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    private void WriteToChannel(ref ReadOnlySequence<byte> buffer)
    //    {
    //        string str = null;

    //        if (buffer.IsSingleSegment)
    //        {
    //            var span = buffer.FirstSpan;
    //            int consumed;
    //            while (span.Length > 0)
    //            {
    //                var newLine = span.IndexOf(NewLine);

    //                if (newLine == -1) break;

    //                var line = span.Slice(0, newLine);
    //                str = Encoding.UTF8.GetString(line);

    //                // write to the channel
    //                _ = _channel.Writer.WriteAsync(new MyData { Content = str, No = ++_counter });

    //                consumed = line.Length + NewLine.Length;
    //                span = span.Slice(consumed);
    //                buffer = buffer.Slice(consumed);
    //            }
    //        }
    //        else
    //        {
    //            var sequenceReader = new SequenceReader<byte>(buffer);

    //            while (!sequenceReader.End)
    //            {
    //                while (sequenceReader.TryReadTo(out ReadOnlySequence<byte> line, NewLine))
    //                {
    //                    str = Encoding.UTF8.GetString(line);

    //                    // write to the channel
    //                    _ = _channel.Writer.WriteAsync(new MyData { Content = str, No = ++_counter });
    //                }

    //                buffer = buffer.Slice(sequenceReader.Position);
    //                sequenceReader.Advance(buffer.Length);
    //            }
    //        }
    //    }
    //}

    //new class ChannelReader
    //{
    //    private const int ChannelReaderCount = 3;

    //    public override void IterationSetup()
    //    {
    //        // ...

    //        _channel = Channel.CreateUnbounded<MyData>(new UnboundedChannelOptions() { SingleReader = ChannelReaderCount == 1 });
    //        for (var i = 0; i < ChannelReaderCount; i++)
    //        {
    //            _channelReaderTasks.Add(DoProcessLine(async (s) => _ = await ProcessLineCoreAsync(s).ConfigureAwait(false)));
    //        }
    //    }

    //    private async Task DoProcessLine(Func<MyData, Task<string>> func)
    //    {
    //        var channelReader = _channel.Reader;
    //        await foreach (var item in channelReader.ReadAllAsync().ConfigureAwait(false))
    //        {
    //            _ = await func(item).ConfigureAwait(false);
    //        }
    //    }
    //}

    //public class MyData
    //{
    //    public string Content { get; set; }
    //    public int No { get; set; }
    //}
}