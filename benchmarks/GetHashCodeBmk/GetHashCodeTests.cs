using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace GetHashCodeBmk
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net90)]
    [MemoryDiagnoser]
    public class GetHashCodeTests
    {
        [Params(1000, 10000, 100000)]
        public int StringLength { get; set; }
        
        [Benchmark(Baseline = true)]
        public int HashOnTheSameInstance_100()
        {
            var s = new string('A', 100);
            return LoopNoInline_GetHashCodeFromString(s);
        }
        
        [Benchmark]
        public int HashOnTheSameInstance()
        {
            var s = new string('A', StringLength);
            return LoopNoInline_GetHashCodeFromString(s);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private int LoopNoInline_GetHashCodeFromString(string s)
        {
            var hash = 0;
            for (int i = 0; i < 1000; i++)
                hash += s.GetHashCode();
            return hash;
        }
    }
}