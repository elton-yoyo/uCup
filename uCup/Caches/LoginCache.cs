using System;
using Microsoft.Extensions.Caching.Memory;
using uCup.Controllers;

namespace uCup.Caches
{
    public interface ILoginCache
    {
        AccountLoginResponse UpdateLoginCache();
        bool IsUserLogin(string loginKey);
    }

    public class LoginCache : ILoginCache
    {
        private readonly IMemoryCache _loginCache;
        private readonly MemoryCacheEntryOptions expiredTime = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
            TimeSpan.FromMinutes(30));

        public LoginCache(IMemoryCache loginCache)
        {
            _loginCache = loginCache;
        }

        public AccountLoginResponse UpdateLoginCache()
        {
            var loginKey = Guid.NewGuid().ToString();
            _loginCache.Set(loginKey, true, expiredTime);
            return new AccountLoginResponse()
            {
                Key = loginKey
            };
        }

        public bool IsUserLogin(string loginKey)
        {
            return _loginCache.TryGetValue(loginKey, out var t);
        }
    }
}