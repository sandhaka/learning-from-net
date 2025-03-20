using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using TransparentCache;

namespace TransparentCacheBmk
{
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class TransparentCacheBmk
    {
        private static IEnumerable<string> StringsCodes => RandomStringUtils.RepeatsRandomly();

        [Params(1000, 10000, 100000)] public int Repeats { get; set; }

        [Benchmark(Baseline = true)] public string[] NoNeedsForCache()
        {
            var received = new string[Repeats];
            using var enumerator = StringsCodes.GetEnumerator();
            for (var i = 0; i < Repeats; i++)
            {
                enumerator.MoveNext();
                received[i] = new string(enumerator.Current);
            }

            return received;
        }

        [Benchmark] public string[] NeedsForCache()
        {
            var received = new string[Repeats];
            using var enumerator = StringsCodes.GetEnumerator();
            for (var i = 0; i < Repeats; i++)
            {
                enumerator.MoveNext();
                var cached = TransparentCaching.GetCached(enumerator.Current);
                received[i] = cached;
            }

            return received;
        }
    }
}
