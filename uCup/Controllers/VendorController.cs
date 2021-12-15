using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uCup.Models;

namespace uCup.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        public VendorResponse Borrow(VendorRequest request)
        {
            return new VendorResponse();
        }
    }

    public class VendorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
