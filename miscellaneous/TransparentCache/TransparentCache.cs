namespace TransparentCache
{
    public class TransparentCache<T> where T : IEquatable<T>
    {
        private readonly HashSet<T> _knowItems = [];

        public T GetCached(T item)
        {
            if (!_knowItems.TryGetValue(item, out var value))
            {
                _knowItems.Add(item);
                value = item;
            }

            return value;
        }
    }

    public class TransparentCache
    {
        private readonly Dictionary<Type, object> _caches = new();

        public T GetCached<T>(T item) where T : IEquatable<T> =>
            GetCache<T>().GetCached(item);

        private TransparentCache<T> GetCache<T>() where T : IEquatable<T>
        {
            if (_caches.TryGetValue(typeof(T), out var cache) && cache is TransparentCache<T> found)
                return found;

            var c = new TransparentCache<T>();
            _caches[typeof(T)] = c;
            return c;
        }
    }

    public static class TransparentCaching
    {
        [ThreadStatic] private static TransparentCache? _transparentCache;

        public static T GetCached<T>(T item) where T : IEquatable<T>
        {
            _transparentCache ??= new TransparentCache();
            return _transparentCache.GetCached(item);
        }
    }
}
