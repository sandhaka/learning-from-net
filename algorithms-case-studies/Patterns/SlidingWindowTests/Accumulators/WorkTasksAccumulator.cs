using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlidingWindowSample.Data;
using SlidingWindowSample.SW;

namespace SlidingWindowTests.Accumulators
{
    public class WorkTasksAccumulator : IAccumulator<WorkTask>
    {
        public int CompareTo(object? obj)
        {
            if (obj is not WorkTasksAccumulator wtaObj)
                return 1;
            if (wtaObj.Value.Effort > this.Value.Effort)
                return -1;
            if (wtaObj.Value.Effort < this.Value.Effort)
                return 1;
            return 0;
        }

        public bool Equals(IAccumulator<WorkTask>? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.Value == Value;
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is not WorkTasksAccumulator workTasksAccumulator)
                return false;
            return Equals(workTasksAccumulator);
        }

        public WorkTask Value { get; private set; } = WorkTask.Empty;

        public void Process(Span<WorkTask> currentWindow)
        {
            var enumerator = currentWindow.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var task = enumerator.Current;
                Value += task;
            }
        }
    }
}
