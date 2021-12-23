using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly string NFC_regexp = "^[A-F0-9]{8}$";

        public VendorController(IUCupProxy uCupProxy)
        {
            _uCupProxy = uCupProxy;
        }

        [HttpGet("GetToken")]
        public async Task<string> GetToken()
        {
            var token =
                await _uCupProxy.GetTokenAsync(new Account("0900000000", "choosebetterbebetter"));
            return token;
        }

        [HttpPost("Register")]
        public async Task<VendorResponse> Register(RegisterRequest request)
        {
            var response = await _uCupProxy.Register(request);
            return new VendorResponse()
            {
                StatusCode = response.Success ? 1 : 0,
                Message = response.Result
            };
        }

        [HttpPost("Rent")]
        public async Task<VendorResponse> Rent(VendorRequest request)
        {
            if (!Regex.IsMatch(request.UniqueId, NFC_regexp))
            {
                return new VendorResponse()
                {
                    StatusCode = 500,
                    Message = "Wrong NFC Id"
                };
            }

            var response = await _uCupProxy.Rent(request);
            return new VendorResponse()
            {
                StatusCode = response.Success ? 1 : 0,
                Message = response.Result
            };
        }

        [HttpPost("Return")]
        public async Task<VendorResponse> Return(VendorRequest request)
        {
            if (!Regex.IsMatch(request.UniqueId, NFC_regexp))
            {
                return new VendorResponse()
                {
                    StatusCode = 500,
                    Message = "Wrong NFC Id"
                };
            }

            var response = await _uCupProxy.Return(request);
            return new VendorResponse()
            {
                StatusCode = response.Success ? 1 : 0,
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