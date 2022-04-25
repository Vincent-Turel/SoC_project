using System;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace ProxyCache
{
    internal class ProxyCache<T>
    {
        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly Func<string, Task<T>> _handler;

        public ProxyCache(Func<string, Task<T>> handler)
        {
            _handler = handler;
        }

        public async Task<T> Get(string nameOfItem) => 
            await Get(nameOfItem, ObjectCache.InfiniteAbsoluteExpiration);

        public async Task<T> Get(string nameOfItem, double dt_seconds) =>
            await Get(nameOfItem, DateTimeOffset.Now.AddSeconds(dt_seconds));

        public async Task<T> Get(string nameOfItem, DateTimeOffset dt)
        {
            if (_cache.Get(nameOfItem) is T x)
            {
                Console.WriteLine("Found item [{0}] in the cache.", nameOfItem);
                return x;
            }

            T value = await _handler(nameOfItem);
            _cache.Add(nameOfItem, value, dt);
            return value;
        }
    }
}