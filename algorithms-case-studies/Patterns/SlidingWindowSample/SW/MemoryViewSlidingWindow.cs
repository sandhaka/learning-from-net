using System.Collections;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;

namespace SlidingWindowSample.SW
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

        internal MemoryViewSlidingWindow(IEnumerable<T> sequence)
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
            return _sequenceMemoryView.Slice(_tailIndex, _headIndex - _tailIndex + 1);
        }
    }
}
