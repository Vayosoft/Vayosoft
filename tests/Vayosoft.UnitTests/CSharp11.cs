using System.Text;
using Xunit.Abstractions;

namespace Vayosoft.UnitTests
{
    public class CSharp11
    {
        private readonly ITestOutputHelper _output;

        public CSharp11(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void RawStringLiterals()
        {
            var xml = $"""
                             <?xml version="1.1" encoding="UTF-8" ?> 
                             <header>{ 1_000_000} </header>     
                             """ ;

            var json = $$"""   
                             {
                              "header": {{ 1_000_000}} 
                             }
                             """ ;


            _output.WriteLine(xml + "\r\n\r\n" + json);
        }

        [Fact]
        public void ListPatterns()
        {
            int[] numbers = {1, 2, 3};

            _output.WriteLine($"{numbers is [1, 2, 3]}");
            _output.WriteLine($"{numbers is [1, 2, 4]}");
            _output.WriteLine($"{numbers is [0 or 1, <= 2, >= 3]}");

            if (numbers is [var first, ..var rest])
            {
                _output.WriteLine($"{first}=>{rest[0]}");
            }

            //-----------------------------------------------
            var emptyName = Array.Empty<string>();
            var myName = new[] {"Anton Beer"};
            var myNameBrokenDown = new[] {"Anton", "Beer"};
            var myNameBrokenDownFurther = new[] {"Anton", "Beer", "The 2nd"};

            var text = emptyName switch
            {
                [] => "Name is empty",
                [var fullName] => $"My full name is: {fullName}",
                [var firstName, var lastName] => $"My full name is {firstName} {lastName}"
            };

            _output.WriteLine(text);
        }

        [Fact]
        public void PatternMachSpan()
        {
            ReadOnlySpan<char> text = "Test String";

            if (text is "Test String")
            {
                _output.WriteLine("is equal");
            }

            if (text is ['T', ..])
            {
                _output.WriteLine("Start with T");
            }
        }

    }


}