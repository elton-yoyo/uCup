using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uCup.Models;

namespace uCup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase
    {
        [HttpPost("Borrow")]
        public VendorResponse Borrow(VendorRequest request)
        {
            return new VendorResponse();
        }

        [HttpPost("VendorReturn")]
        public VendorResponse VendorReturn(VendorRequest request)
        {
            return new VendorResponse();
        }
    }
}
