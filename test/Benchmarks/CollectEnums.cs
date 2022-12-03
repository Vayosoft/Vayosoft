using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class CollectEnums
{
    [Params(1000, 10000, 100000, 1000000)] public int N;

    [Benchmark]
    public EnumFromInt[] EnumOfInt()
    {
        EnumFromInt[] results = new EnumFromInt[N];
        for (int i = 0; i < N; i++)
        {
            results[i] = EnumFromInt.Value1;
        }

        return results;
    }

    [Benchmark]
    public EnumFromByte[] EnumOfByte()
    {
        EnumFromByte[] results = new EnumFromByte[N];
        for (int i = 0; i < N; i++)
        {
            results[i] = EnumFromByte.Value1;
        }

        return results;
    }
}

public enum EnumFromInt
{
    Value1,
    Value2
}

public enum EnumFromByte : byte
{
    Value1,
    Value2
}