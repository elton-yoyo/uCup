using System;
using Microsoft.Extensions.Caching.Memory;

namespace uCup.Caches
{
    public class RentStatusCache : IRentStatusCache
    {
        private readonly IMemoryCache _rentCache;
        private readonly MemoryCacheEntryOptions expiredTime = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
            TimeSpan.FromMinutes(3));
        public RentStatusCache(IMemoryCache rentCache)
        {
            _rentCache = rentCache;
        }
        public void InsertCache(string userId)
        {
            _rentCache.Set(userId, true, expiredTime);
        }
        
        public void RemoveCache(string userId)
        {
            _rentCache.Remove(userId);
        }

        public bool GetCache(string userId)
        {
            return _rentCache.Get(userId) != null;
        }
    }

    public interface IRentStatusCache
    {
        bool GetCache(string userId);
        void RemoveCache(string userId);
        void InsertCache(string userId);
    }
}