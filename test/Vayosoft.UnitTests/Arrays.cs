using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Vayosoft.UnitTests
{
    public class Arrays
    {
        private readonly ITestOutputHelper _output;

        public Arrays(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BinarySearch()
        {
            var array = new int[] {1, 2, 3, 4, 5};
            var span = new Span<int>(array);

            var position = span.BinarySearch(3);
            _output.WriteLine(position.ToString());


        }
    }
}
