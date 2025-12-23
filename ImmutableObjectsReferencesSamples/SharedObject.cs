using System.Collections.Immutable;

namespace ImmutableObjectsReferencesSamples
{
    internal class SharedObject
    {
        public SharedObject(ImmutableArray<int> values) { Values = values; }

        public ImmutableArray<int> Values { get; }

        public override string ToString() => string.Format($"{string.Join(",", Values)}");
    }
}
