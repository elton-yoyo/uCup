using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uCup.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        public PlatformResponse Rent(PlatformRequest request)
        {
            return new PlatformResponse();
        }

        public PlatformResponse Return(PlatformRequest request)
        {
            return new PlatformResponse();
        }
    }
    

    public class PlatformResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

    public class PlatformRequest
    {
        public string UniqueId { get; set; }
        public string MerchantCode { get; set; }
        public DateTime Time { get; set; }
    }
}
