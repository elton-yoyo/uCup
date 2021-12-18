using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uCup.Caches;
using uCup.Models;

namespace uCup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase
    {
        /// <summary>
        ///     租借
        /// </summary>
        /// <returns>statusCode: 0=失敗 1=成功 2=處理中</returns>
        [HttpPost("Rent")]
        public VendorResponse Rent(VendorRequest request)
        {
            var input = new PlatformRequest()
            {
                UniqueId = request.UniqueId,
                MerchantCode = request.MerchantCode,
                Time = DateTime.Now
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
                Time = DateTime.Now
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
