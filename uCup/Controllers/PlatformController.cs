using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using uCup.Caches;
using uCup.Models;
using uCup.Services;

namespace uCup.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;
        private readonly ILoginCache _loginCache;

        public PlatformController(IHealthCheckService healthCheckService, ILoginCache loginCache)
        {
            _healthCheckService = healthCheckService;
            _loginCache = loginCache;
        }

        [HttpPost("Rent")]
        public PlatformResponse Rent(PlatformRequest request)
        {
            return new PlatformResponse();
        }

        [HttpPost("Return")]
        public PlatformResponse Return(PlatformRequest request)
        {
            return new PlatformResponse();
        }

        [HttpGet("getrentdata")]
        public object GetRentData()
        {
            return JsonConvert.SerializeObject(VenderCache.GetRentCache());
        }

        [HttpGet("getreturndata")]
        public object GetReturnData()
        {
            return JsonConvert.SerializeObject(VenderCache.GetReturnCache());
        }
        
        [HttpGet("getallmachine")]
        public async Task<List<LiveRequest>> GetAllMachine(string loginKey)
        {
            if (_loginCache.IsUserLogin(loginKey))
                return await _healthCheckService.GetCache();

            return new List<LiveRequest>();
        }

        [HttpGet("clear")]
        public void Clear()
        {
            VenderCache.ClearCache();
        }
    }
}
