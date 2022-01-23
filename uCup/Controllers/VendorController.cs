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
        private readonly string _nfcRegexp = "^[A-F0-9]{8}$";

        public VendorController(IUCupProxy uCupProxy)
        {
            _uCupProxy = uCupProxy;
        }

        [HttpGet("GetToken")]
        public async Task<string> GetToken()
        {
            var token = await _uCupProxy.GetTokenAsync(new Account("0900000000", "choosebetterbebetter"));
            //var token = await _uCupProxy.GetTokenAsync(new Account("0922441389", "121537853"));
            return token;
        }

        [HttpPost("Register")]
        public async Task<VendorResponse> Register(RegisterRequest request)
        {
            var response = await _uCupProxy.Register(request);
            return new VendorResponse()
            {
                ErrorCode = response.ErrorCode,
                Message = response.Result
            };
        }

        [HttpPost("Rent")]
        public async Task<VendorResponse> Rent(VendorRequest request)
        {
            // if (!Regex.IsMatch(request.UniqueId, NFC_regexp))
            // {
            //     return new VendorResponse()
            //     {
            //         ErrorCode = 500,
            //         Message = "Wrong NFC Id"
            //     };
            // }
            
            
            var rentRequest = new VendorRequest()
            {
                UniqueId = request.UniqueId,
                Password = request.Password,
                Phone = request.Phone,
                Provider = request.Provider,
                Type = request.Type
            };

            var response = await _uCupProxy.Rent(rentRequest);
            if (response.ErrorCode == 2)
            {
                response = await _uCupProxy.Return(rentRequest);
                return new VendorResponse()
                {
                    ErrorCode = response.ErrorCode != 0 ? response.ErrorCode : 9001,
                    Message = response.ErrorCode != 0 ? response.Result : "Return Success"
                };
            }
            return new VendorResponse()
            {
                ErrorCode = response.ErrorCode,
                Message = response.Result
            };
        }

        [HttpPost("Return")]
        public async Task<VendorResponse> Return(VendorRequest request)
        {
            // if (!Regex.IsMatch(request.UniqueId, NFC_regexp))
            // {
            //     return new VendorResponse()
            //     {
            //         ErrorCode = 500,
            //         Message = "Wrong NFC Id"
            //     };
            // }
            var returnRequest = new VendorRequest()
            {
                UniqueId = request.UniqueId,
                Password = request.Password,
                Phone = request.Phone,
                Provider = request.Provider,
                Type = request.Type
            };
            var response = await _uCupProxy.Return(returnRequest);
            return new VendorResponse()
            {
                ErrorCode = response.ErrorCode,
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