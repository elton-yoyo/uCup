using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using uCup.Caches;
using uCup.Models;

namespace uCup.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class PlatformController : ControllerBase
    {
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

        [HttpGet("getall")]
        public object GetAll()
        {
            return JsonConvert.SerializeObject(VenderCache.GetAllCache());
        }
    }
}
