using System.Collections;
using System.Diagnostics.Contracts;

namespace SlidingWindowSample.SW
{
    internal class SlidingWindow<T> : ISlidingWindow<T>
    {
        private readonly T[] _sequence;
        private Memory<T> _memory;

        internal SlidingWindow(IEnumerable<T> sequence)
        {
            _sequence = sequence.ToArray();
            Init();
        }

        internal SlidingWindow(T[] sequence, int start = 0, int length = 1)
        {
            _sequence = sequence;
            Init(start, length);
        }

        public int TailIndex { get; private set; }

        public int HeadIndex { get; private set; }

        public T Head => _memory.Span[HeadIndex];

        public T Tail => _memory.Span[TailIndex];

        public int Length => _memory.Length;

        public Func<T, T, bool> RemoveHeadPredicate => throw new NotImplementedException();

        public void Advance(int count)
        {
            Contract.Assume(count > 0);

            if (HeadIndex + count >= _sequence.Length) 
                throw new InvalidOperationException();

            TailIndex += count;
            HeadIndex += count;

            _memory = new Memory<T>(_sequence, TailIndex, HeadIndex - TailIndex);
        }

        public void FallBack(int count)
        {
            Contract.Assume(count > 0);

            if (TailIndex - count < 0)
                throw new InvalidOperationException();

            HeadIndex -= count;
            TailIndex -= count;

            _memory = new Memory<T>(_sequence, TailIndex, HeadIndex - TailIndex);
        }

        private void Init(int start = 0, int length = 1)
        {
            Contract.Assume(_sequence.Length > 0);
            Contract.Assume(start >= 0);
            Contract.Assume(length > 0);

            TailIndex = start;
            HeadIndex = start + length - 1;

            if (TailIndex < 0 || HeadIndex < TailIndex || HeadIndex >= _sequence.Length) 
                throw new InvalidOperationException("Wrong slice parameters");

            _memory = new Memory<T>(_sequence, start, length);
        }
    }
}
