using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using uCup.Models;

namespace uCup.Caches
{
    public class VenderCache
    {
        private static Dictionary<string, PlatformRequest> cacheData = new Dictionary<string, PlatformRequest>();
        public static void InsertCache(PlatformRequest input)
        {
            cacheData.Add(input.UniqueId, input);
        }

        public static IEnumerable<PlatformRequest> GetAllCache()
        {
            return cacheData.Select(x=>x.Value);
        }

        public static void RemoveCache(PlatformRequest input)
        {
            cacheData.Remove(input.UniqueId);
        }
    }
}
