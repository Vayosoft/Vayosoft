using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Vayosoft.Utilities;

//https://sharplab.io
namespace Benchmarks;

[MemoryDiagnoser(true)]
public class Enumeration
{
    private static readonly Random R = new(80085);
    private List<int> _list;

    [Params(100, 100_000, 1_000_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _list = Enumerable.Range(1, Size).Select(r => R.Next(r)).ToList();
    }

    [Benchmark]
    public void For()
    {
        for (var i = 0; i < _list.Count; i++)
        {
            var item = _list[i];
        }
    }

    [Benchmark]
    public void CustomForeach()
    {
        foreach (var i in .._list.Count)
        { }
    }

    [Benchmark]
    public void Foreach()
    {
        foreach (var i in _list)
        { }
    }

    [Benchmark]
    public void Foreach_Linq()
    {
        _list.ForEach(i => {});
    }

    [Benchmark]
    public void Foreach_Parallel()
    {
        Parallel.ForEach(_list, i => { });
    }

    [Benchmark]
    public void Parallel_Linq()
    {
        _list.AsParallel().ForAll(i => { });
    }

    [Benchmark]
    public void For_Span()
    {
        var asSpan = CollectionsMarshal.AsSpan(_list);
        for (var i = 0; i < asSpan.Length; i++)
        {
            var item = asSpan[i];
            Do(item);
        }
    }

    [Benchmark]
    public void Foreach_Span()
    {
        foreach (var i in CollectionsMarshal.AsSpan(_list))
        {
            Do(i);
        }
    }

    [Benchmark]
    public void For_SpanV2()
    {
        var asSpan = CollectionsMarshal.AsSpan(_list);
        ref var searchSpace = ref MemoryMarshal.GetReference(asSpan);
        for (var i = 0; i < asSpan.Length; i++)
        {
            var item = Unsafe.Add(ref searchSpace, i);
            Do(item);
        }
    }

    private void Do(int i){}
}