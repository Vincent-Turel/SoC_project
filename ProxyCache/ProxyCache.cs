using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Threading;

namespace ProxyCache
{
    internal class ProxyCache<T>
    {
        
        private readonly ObjectCache _cache = MemoryCache.Default;
        
        public ProxyCache() {}

        public T Get(string nameOfItem)
        {
            return (T) _cache.AddOrGetExisting(nameOfItem, Activator.CreateInstance(typeof(T), nameOfItem), ObjectCache.InfiniteAbsoluteExpiration);
        }
        
        public T Get(string nameOfItem, double dt_seconds)
        {
            return (T) _cache.AddOrGetExisting(nameOfItem, Activator.CreateInstance(typeof(T), nameOfItem), 
                new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(dt_seconds) });
        }
        
        public T Get(string nameOfItem, DateTimeOffset dt)
        {
            return (T) _cache.AddOrGetExisting(nameOfItem, Activator.CreateInstance(typeof(T), nameOfItem), 
                new CacheItemPolicy { AbsoluteExpiration = dt});
        }
        
        private void PrintAllCache(ObjectCache cache)
        {

            DateTime dt = DateTime.Now;

            Console.WriteLine("All key-values at " + dt.Second);
            //loop through all key-value pairs and print them
            foreach (var item in cache)
            {
                Console.WriteLine("cache object key-value: " + item.Key + "-" + item.Value);
            }
        }
    }
}
