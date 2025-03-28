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
    }
}
