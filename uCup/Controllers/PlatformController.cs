using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uCup.Models;

namespace uCup.Controllers
{
    [ApiController]
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
    }
}
