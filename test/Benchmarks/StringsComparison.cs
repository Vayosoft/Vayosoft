using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class StringsComparison
    {
        public const string DefaultString = "VXTDuob5YhummuDq1PPXOHE4PbrRjYfBjcHdFs8UcKSAHOCGievbUItWhU3ovCmRALgdZUG1CB0sQ4iMj8Z1ZfkML2owvfkOKxBCoFUAN4VLd4I8ietmlsS5PtdQEn6zEgy1uCVZXiXuubd0xM5ONVZBqDu6nOVq1GQloEjeRN8jXrj0MVUexB9aIECs7caKGddpuut3";

        [Benchmark]
        public bool ToLower()
        {
            return DefaultString.ToLower() == DefaultString.ToLower();
        }

        
        [Benchmark]
        public bool ToLowerInvariant()
        {
            return DefaultString.ToLowerInvariant() == DefaultString.ToLowerInvariant();
        }

       
        [Benchmark]
        public bool ToUpper()
        {
            return DefaultString.ToUpper() == DefaultString.ToUpper();
        }

        [Benchmark]
        public bool ToUpperInvariant()
        {
            return DefaultString.ToUpperInvariant() == DefaultString.ToUpperInvariant();
        }

        [Benchmark]
        public bool Equals_Ordinal()
        {
            return DefaultString.Equals(DefaultString, StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public bool Equals_Invariant()
        {
            return DefaultString.Equals(DefaultString, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
