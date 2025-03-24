using System.Collections;

namespace SlidingWindowSample.SW
{
    internal class SlidingWindow<T> : ISlidingWindow<T>, IEnumerable<T>
    {
        public T Head => throw new NotImplementedException();

        public T Tail => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public Func<T, T, bool> RemoveHead => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
