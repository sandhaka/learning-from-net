using BenchmarkDotNet.Running;

namespace GetHashCodeBmk
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<GetHashCodeTests>();
        }
    }
}