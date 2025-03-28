using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingWindowSample.SW
{
    internal class QueueSlidingWindow<T> : ISlidingWindow<T>
    {
        public T Head { get; }
        public T Tail { get; }
        public int Length { get; }
        public Func<T, T, bool> RemoveHeadPredicate { get; }
        public void Advance(int count)
        {
            throw new NotImplementedException();
        }

        public void FallBack(int count)
        {
            throw new NotImplementedException();
        }
    }
}
