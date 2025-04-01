using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SlidingWindowSample.SW;

namespace SlidingWindowBmk
{
    [MemoryDiagnoser(displayGenColumns: true)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class SlidingPerf
    {
        [Params(10, 100)] public int WindowSize { get; set; }
        [Params(10000000)] public int Length { get; set; }

        private readonly IEnumerable<int> _enumerable = Enumerable.Range(0, 10000000);
        private readonly int[] _arr = Enumerable.Range(0, 10000000).ToArray();
        private readonly IAccumulator<int> _accumulator = new SimpleAccumulator();

        private ISlidingWindow<int> _memorySlidingWindow;
        private ISlidingWindow<int> _queueSlidingWindow;

        private class SimpleAccumulator : IAccumulator<int>
        {
            public int CompareTo(object? obj)
            {
                if (obj is not SimpleAccumulator wtaObj)
                    return 1;
                if (wtaObj.Value > this.Value)
                    return -1;
                if (wtaObj.Value < this.Value)
                    return 1;
                return 0;
            }

            public bool Equals(IAccumulator<int>? other)
            {
                if (other == null) return false;
                if (ReferenceEquals(this, other))
                    return true;
                return other.Value == Value;
            }

            public int Value { get; private set; } = 0;
            public void Process(int current) => Value += current;
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _memorySlidingWindow = SlidingWindowFactory.Create(_arr, 0, WindowSize);
            _queueSlidingWindow = SlidingWindowFactory.Create(_enumerable, 0, WindowSize);
            _memorySlidingWindow.AddAccumulator(_accumulator);
            _queueSlidingWindow.AddAccumulator(_accumulator);
        }

        [Benchmark(Description = "Memory<> implementation")]
        public void MemorySlidingWindow()
        {
            for (var i = 0; i < Length - WindowSize; i++)
                _memorySlidingWindow.Advance(1);
        }

        [Benchmark(Description = "Queue<> implementation")]
        public void QueueSlidingWindow()
        {
            for (var i = 0; i < Length - WindowSize; i++)
                _queueSlidingWindow.Advance(1);
        }
    }
}
