namespace SlidingWindowSample.SW
{
    public interface ISlidingWindow<T>
    {
        public T Head { get; }
        public T Tail { get; }
        public int Count { get; }
        public Func<T, T, bool> RemoveHead { get; }
    }
}
