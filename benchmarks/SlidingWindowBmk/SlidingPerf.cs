using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SlidingWindowSample.SW;

namespace SlidingWindowBmk
{
    [MemoryDiagnoser(displayGenColumns: true)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class SlidingPerf
    {
        [Params(10, 100, 1000)] private int _windowSize;
        [Params(1000, 10000, 100000, 1000000)] private int _length;

        private readonly IEnumerable<int> _enumerable = Enumerable.Range(0, 1000000);
        private readonly int[] _arr = Enumerable.Range(0, 1000000).ToArray();
        private readonly ISlidingWindow<int> _memorySlidingWindow;
        private readonly ISlidingWindow<int> _queueSlidingWindow;

        public SlidingPerf()
        {
            _memorySlidingWindow = SlidingWindowFactory.Create(_arr, 0, _windowSize);
            _queueSlidingWindow = SlidingWindowFactory.Create(_enumerable, 0, _windowSize);
        }

        [Benchmark(Description = "Memory<> implementation")]
        public void MemorySlidingWindow()
        {

        }

        [Benchmark(Description = "Queue<> implementation")]
        public void QueueSlidingWindow()
        {

        }
    }
}
