using BenchmarkDotNet.Running;

namespace ActionMethodsBmk
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<ActionsBmk>();
        }
    }
}