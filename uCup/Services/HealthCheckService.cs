using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using uCup.Models;

namespace uCup.Services
{
    public interface IHealthCheckService
    {
        Task<bool> UpdateCache(LiveRequest request);
        Task<List<LiveRequest>> GetCache();
    }

    public class HealthCheckService : IHealthCheckService
    {
        private readonly IMemoryCache _aliveCache;

        private readonly ConcurrentDictionary<string, LiveRequest> _machineCache =
            new ConcurrentDictionary<string, LiveRequest>();

        public HealthCheckService(IMemoryCache aliveCache)
        {
            _aliveCache = aliveCache;
        }

        public async Task<bool> UpdateCache(LiveRequest request)
        {
            try
            {
                // var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
                //     TimeSpan.FromHours(6));
                //
                // _aliveCache.Set(request.MachineName, request, cacheEntryOptions);
                _machineCache.AddOrUpdate(request.MachineName, request, (key, liveRequest) => request);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<LiveRequest>> GetCache()
        {
            var list = new List<LiveRequest>();

            foreach (var cache in _machineCache)
            {
                list.Add(cache.Value);
            }

            return list.OrderBy(x=>x.RequestTime).ToList();
        }
    }
}