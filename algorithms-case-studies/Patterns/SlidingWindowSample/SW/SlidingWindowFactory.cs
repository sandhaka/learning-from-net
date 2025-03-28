namespace SlidingWindowSample.SW
{
    public static class SlidingWindowFactory
    {
        public static ISlidingWindow<T> Create<T>(T[] sequence) =>
            new MemoryViewSlidingWindow<T>(sequence);

        public static ISlidingWindow<T> Create<T>(T[] sequence, int start) =>
            new MemoryViewSlidingWindow<T>(sequence, start);

        public static ISlidingWindow<T> Create<T>(T[] sequence, int start, int length) =>
            new MemoryViewSlidingWindow<T>(sequence, start, length);

        public static ISlidingWindow<T> Create<T>(IEnumerable<T> enumerable) =>
            new QueueSlidingWindow<T>(enumerable);

        public static ISlidingWindow<T> Create<T>(IEnumerable<T> enumerable, int start) =>
            new QueueSlidingWindow<T>(enumerable, start);

        public static ISlidingWindow<T> Create<T>(IEnumerable<T> enumerable, int start, int length) =>
            new QueueSlidingWindow<T>(enumerable, start, length);
    }
}
