using System.Diagnostics.Contracts;

namespace SlidingWindowSample.SW.Implementations
{
    internal class QueueSlidingWindow<T> : ISlidingWindow<T>, IDisposable
    {
        private readonly IEnumerator<T> _seqEnumerator;
        private readonly Queue<T> _queue = new Queue<T>();
        private T _lastAppended = default!;

        private readonly HashSet<IAccumulator<T>> _accumulators = [];

        internal QueueSlidingWindow(IEnumerable<T> enumerable, int start = 0, int length = 1)
        {
            if (length <= 0) throw new InvalidOperationException();

            _seqEnumerator = enumerable.Skip(start).GetEnumerator();
            for (var i = 0; i < length; i++)
            {
                if (_seqEnumerator.MoveNext() is false)
                    throw new InvalidOperationException();

                _queue.Enqueue(_seqEnumerator.Current);
                _lastAppended = _seqEnumerator.Current;
            }
        }

        public T Head => _queue.Last();
        
        public T Tail => _queue.Peek();

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
                _queue.Dequeue();
            }

            foreach (var accumulator in _accumulators)
            {
                foreach (var item in _queue)
                    accumulator.Process(item);
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
