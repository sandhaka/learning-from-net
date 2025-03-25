namespace SlidingWindowSample.SW
{
    public static class SlidingWindowFactory
    {
        public static ISlidingWindow<T> Create<T>(T[] sequence) =>
            new SlidingWindow<T>(sequence);

        public static ISlidingWindow<T> Create<T>(T[] sequence, int start) =>
            new SlidingWindow<T>(sequence, start);

        public static ISlidingWindow<T> Create<T>(T[] sequence, int start, int length) =>
            new SlidingWindow<T>(sequence, start, length);
    }
}
