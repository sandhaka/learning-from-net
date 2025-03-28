namespace SlidingWindowSample.SW
{
    internal class QueueSlidingWindow<T> : ISlidingWindow<T>, IDisposable
    {
        private readonly IEnumerator<T> _seqEnumerator;
        private readonly Queue<T> _queue = new Queue<T>();

        internal QueueSlidingWindow(IEnumerable<T> enumerable)
        {
            _seqEnumerator = enumerable.GetEnumerator();
        }

        internal QueueSlidingWindow(IEnumerable<T> enumerable, int start, int length = 1)
        {
            _seqEnumerator = enumerable.GetEnumerator();
        }

        public T Head => _queue.Peek();
        
        public T Tail => _queue.Last();
        
        public int Length => _queue.Count;

        public Func<T, T, bool> RemoveHeadPredicate => throw new NotImplementedException();

        public void Advance(int count)
        {
            throw new NotImplementedException();
        }

        public void FallBack(int count)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _seqEnumerator.Dispose();
        }
    }
}
