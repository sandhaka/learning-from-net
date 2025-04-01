using System.Diagnostics.Contracts;

namespace SlidingWindowSample.SW.Implementations
{
    internal class MemoryViewSlidingWindow<T> : ISlidingWindow<T>
    {
        // Original sequence references
        private readonly T[] _sequence;
        private readonly Memory<T> _sequenceMemoryView;

        // Internal memory view
        private Memory<T> _window;

        // Internal indexes
        private int _tailIndex;
        private int _headIndex;

        private readonly HashSet<IAccumulator<T>> _accumulators = [];

        internal MemoryViewSlidingWindow(T[] sequence)
        {
            _sequence = sequence.ToArray();
            _sequenceMemoryView = _sequence;
            _window = Init();
        }

        internal MemoryViewSlidingWindow(T[] sequence, int start = 0, int length = 1)
        {
            _sequence = sequence.ToArray();
            _sequenceMemoryView = _sequence;
            _window = Init(start, length);
        }

        public int TailIndex => _tailIndex;

        public int HeadIndex => _headIndex;

        public T Head => _sequence[_headIndex];

        public T Tail => _sequence[_tailIndex];

        public int Length => _window.Length;

        public Func<T, T, bool> RemoveHeadPredicate => throw new NotImplementedException();

        public void Advance(int count)
        {
            Contract.Assume(count > 0);

            if (_headIndex + count >= _sequenceMemoryView.Length) 
                throw new InvalidOperationException();

            _tailIndex += count;
            _headIndex += count;

            _window = Update();
        }

        public void FallBack(int count)
        {
            Contract.Assume(count > 0);

            if (_tailIndex - count < 0)
                throw new InvalidOperationException();

            _headIndex -= count;
            _tailIndex -= count;

            _window = Update();
        }

        public void AddAccumulator(IAccumulator<T> accumulator)
        {
            Contract.Assume(accumulator != null);

            _accumulators.Add(accumulator);
        }

        private Memory<T> Init(int start = 0, int length = 1)
        {
            Contract.Assume(_sequenceMemoryView.Length > 0);
            Contract.Assume(start >= 0);
            Contract.Assume(length > 0);

            _tailIndex = start;
            _headIndex = start + length - 1;

            return new Memory<T>(_sequence, start, length);
        }

        private Memory<T> Update()
        {
            if (_tailIndex < 0 || _headIndex < _tailIndex || _headIndex >= _sequenceMemoryView.Length)
                throw new InvalidOperationException("Wrong slice parameters");

            var span = _sequenceMemoryView.Span[_tailIndex..(_headIndex + 1)];

            foreach (var accumulator in _accumulators)
            {
                foreach (var item in span)
                    accumulator.Process(item);
            }

            return _sequenceMemoryView.Slice(_tailIndex, _headIndex - _tailIndex + 1);
        }
    }
}
