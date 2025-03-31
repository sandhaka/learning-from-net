using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingWindowSample.SW
{
    public interface IAccumulator<T> : IComparable, IEquatable<IAccumulator<T>>
    {
        T Value { get; }
        void Process(T current);
    }
}
