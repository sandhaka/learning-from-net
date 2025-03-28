namespace SlidingWindowSample.SW
{
    public interface ISlidingWindow<T>
    {
        public T Head { get; }
        public T Tail { get; }
        public int Length { get; }
        public Func<T, T, bool> RemoveHeadPredicate { get; }
        public void Advance(int count);
        public void FallBack(int count);
        public void AddAccumulator(IAccumulator<T> accumulator);
    }
}
