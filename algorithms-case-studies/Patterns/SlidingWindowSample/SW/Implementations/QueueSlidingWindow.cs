using System.Diagnostics.Contracts;

namespace SlidingWindowSample.SW.Implementations
{
    internal class QueueSlidingWindow<T> : ISlidingWindow<T>, IDisposable
    {
        private readonly IEnumerator<T> _seqEnumerator;
        private readonly Queue<T> _queue = new Queue<T>();
        private T _lastAppended = default!;

        private readonly HashSet<IAccumulator<T>> _accumulators = [];

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
            Contract.Assume(count > 0);

            for (var i = 0; i < count; i++)
            {
                if (_seqEnumerator.MoveNext() is false)
                    throw new InvalidOperationException();

                _queue.Enqueue(_seqEnumerator.Current);
                _lastAppended = _seqEnumerator.Current;
            }
        }

        public void FallBack(int count) =>
            throw new InvalidOperationException("Enumerator-based windows cannot go backward.");

        public void AddAccumulator(IAccumulator<T> accumulator)
        {
            Contract.Assume(accumulator != null);

            _accumulators.Add(accumulator);
        }

        public void Dispose()
        {
            _seqEnumerator.Dispose();
        }
    }
}
