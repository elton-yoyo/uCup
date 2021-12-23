using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using uCup.Caches;
using uCup.Models;
using uCup.Proxy;

namespace uCup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase
    {
        [HttpPost("Rent")]
        public VendorResponse Rent(VendorRequest request)
        {
            var input = new PlatformRequest()
            {
                UniqueId = request.UniqueId,
                MerchantCode = request.MerchantCode,
                Time = DateTime.UtcNow.AddHours(8)
            };

            VenderCache.InsertCache(input);
            return new VendorResponse();
        }

        [HttpPost("Return")]
        public VendorResponse Return(VendorRequest request)
        {
            var input = new PlatformRequest()
            {
                UniqueId = request.UniqueId,
                MerchantCode = request.MerchantCode,
                Time = DateTime.UtcNow.AddHours(8)
            };

            VenderCache.RemoveCache(input);
            return new VendorResponse();
        }

        [HttpPost("Alive")]
        public bool Alive(string merchantCode)
        {
            return true;
        }
    }
}
