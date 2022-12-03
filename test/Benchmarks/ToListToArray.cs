using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class ToListToArray
    {
        private List<int> _list;
        private int[] _array;
        private IEnumerable<int> _items;

        [Params(100000, 10000000)] public int N;

        [GlobalSetup]
        public void Setup()
        { 
            const int MIN = 1;
            const int MAX = 10;
            var rnd = new Random();

            _items = Enumerable.Repeat(0, N).Select(i => rnd.Next(MIN, MAX));
            _list = _items.ToList();
            _array = _list.ToArray();
        }

        [Benchmark]
        public List<int> ToList()
        {
            return _list.ToList();
        }

        [Benchmark]
        public int[] ToArray()
        {
            return _list.ToArray();
        }

        [Benchmark]
        public int ForList()
        {
            int total = 0;
            for (int i = 0; i < _list.Count; i++)
            {
                total += _list[i];
            }

            return total;
        }

        [Benchmark]
        public int ForeachList()
        {
            int total = 0;
            foreach (int i in _list)
            {
                total += i;
            }

            return total;
        }

        [Benchmark]
        public int ForeachArray()
        {
            int total = 0;
            foreach (int i in _array)
            {
                total += i;
            }

            return total;
        }

        [Benchmark]
        public int ForArray()
        {
            int total = 0;
            for (int i = 0; i < _array.Length; i++)
            {
                total += _array[i];
            }

            return total;
        }
    }
}
