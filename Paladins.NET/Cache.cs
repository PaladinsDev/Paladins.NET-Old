using System;
using System.Runtime.Caching;

namespace PaladinsDev.PaladinsDotNET
{
    class Cache
    {
        private ObjectCache cache;

        public Cache()
        {
            this.cache = MemoryCache.Default;
        }
        public T Remember<T>(string itemName, int timeToCache, Func<T> setFunction)
        {
            var cacheItem = (T)this.cache[itemName];

            if (cacheItem == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(timeToCache);

                cacheItem = setFunction();
                this.cache.Set(itemName, cacheItem, policy);
            }

            return cacheItem;
        }
    }
}
t