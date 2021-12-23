using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using uCup.Caches;
using uCup.Models;
using uCup.Proxies;

namespace uCup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IUCupProxy _uCupProxy;

        public VendorController(IUCupProxy uCupProxy)
        {
            _uCupProxy = uCupProxy;
        }

        [HttpGet("GetToken")]
        public async Task<string> GetToken()
        {
            var token =
                await _uCupProxy.GetTokenAsync(new LoginRequest("0900000000", "choosebetterbebetter"));
            return token;
        }

        [HttpPost("Rent")]
        public async Task<VendorResponse> Rent(VendorRequest request)
        {
            var response = await _uCupProxy.Rent(request);
            return new VendorResponse()
            {
                StatusCode = response.Success ? 200 : 500,
                Message = response.Result
            };
        }

        [HttpPost("Return")]
        public async Task<VendorResponse> Return(VendorRequest request)
        {
         
            var response = await _uCupProxy.Return(request);
            return new VendorResponse()
            {
                StatusCode = response.Success ? 200 : 500,
                Message = response.Result
            };
        }

        [HttpPost("Alive")]
        public bool Alive(string merchantCode)
        {
            return true;
        }
    }
}