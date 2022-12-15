using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using System.Buffers;
using System.Reflection.PortableExecutable;
using Microsoft.IO;

namespace Benchmarks
{
    [MemoryDiagnoser]
    [Config(typeof(DontForceGcCollectionsConfig))] // we don't want to interfere with GC, we want to include it's impact
    public class Pooling
    {
        [Params((int)1E+2, // 100 bytes
            (int)1E+3, // 1 000 bytes = 1 KB
            (int)1E+4, // 10 000 bytes = 10 KB
            (int)1E+5, // 100 000 bytes = 100 KB
            (int)1E+6, // 1 000 000 bytes = 1 MB
            (int)1E+7)] // 10 000 000 bytes = 10 MB
        public int SizeInBytes { get; set; }

        private ArrayPool<byte> _sizeAwarePool;

        [GlobalSetup]
        public void GlobalSetup()
            => _sizeAwarePool = ArrayPool<byte>.Create(SizeInBytes + 1, 10); // let's create the pool that knows the real max size

        [Benchmark]
        public void Allocate()
            => DeadCodeEliminationHelper.KeepAliveWithoutBoxing(new byte[SizeInBytes]);

        [Benchmark]
        public void RentAndReturn_Shared()
        {
            var pool = ArrayPool<byte>.Shared;
            byte[] array = pool.Rent(SizeInBytes);
            pool.Return(array);
        }

        [Benchmark]
        public void RentAndReturn_Aware()
        {
            var pool = _sizeAwarePool;
            byte[] array = pool.Rent(SizeInBytes);
            pool.Return(array);
        }

        //private readonly RecyclableMemoryStreamManager _manager = new();
        //[Benchmark]
        //public byte[] ReorderPagesRecycle(byte[] pdf, List<int> order)
        //{
        //    using (var inputStream = _manager.GetStream(pdf))
        //    {
        //        using (var reader = new PdfReader(inputStream))
        //        {
        //            using (var outputStream = _manager.GetStream("Reorder", PdfSize))
        //            {
        //                using (var writer = new PdfWriter(outputStream))
        //                {
        //                    using (var inputDocument = new PdfDocument(reader))
        //                    {
        //                        using (var outputDocument = new PdfDocument(writer))
        //                        {
        //                            inputDocument.CopyPagesTo(order, outputDocument);
        //                            return outputStream.GetBuffer();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

    public class DontForceGcCollectionsConfig : ManualConfig
    {
        public DontForceGcCollectionsConfig()
        {
            AddJob(Job.Default
                .WithGcMode(new GcMode()
                {
                    Force = false // tell BenchmarkDotNet not to force GC collections after every iteration
                }));
        }
    }
}
