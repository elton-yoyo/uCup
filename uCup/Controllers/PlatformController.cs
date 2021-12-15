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
        public RentResponse Rent(RentRequest request)
        {

        }
    }

    public class RentRequest
    {
        public string UniqueId { get; set; }
        public string MerchantCode { get; set; }
        public DateTime Time { get; set; }
    }
}
