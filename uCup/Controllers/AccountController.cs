using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using uCup.Caches;

namespace uCup.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILoginCache _loginCache;
        private readonly MemoryCacheEntryOptions expiredTime = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
            TimeSpan.FromMinutes(30));

        public AccountController(ILoginCache loginCache)
        {
            _loginCache = loginCache;
        }

        [HttpPost("Login")]
        public object Login(AccountLoginRequest request)
        {
            if (request.Account == "ucup" && request.Password == "1q2w#E$R")
            {
                return _loginCache.UpdateLoginCache();
            }

            return new AccountLoginResponse();
        }
    }

    public class AccountLoginRequest
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }

    public class AccountLoginResponse
    {
        public string Key { get; set; }
    }
}