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
        private static List<PlatformRequest> rentData = new List<PlatformRequest>();
        private static List<PlatformRequest> returnData = new List<PlatformRequest>();
        public static void InsertCache(PlatformRequest input)
        {
            rentData.Add(input);
        }

        public static IEnumerable<PlatformRequest> GetRentCache()
        {
            return rentData;
        }

        public static IEnumerable<PlatformRequest> GetReturnCache()
        {
            return returnData;
        }

        public static void RemoveCache(PlatformRequest input)
        {
            returnData.Add(input);
        }

        public static void ClearCache()
        {
            returnData.Clear();
            rentData.Clear();
        }
    }
}
