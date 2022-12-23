using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using uCup.Models;
using uCup.Proxies;
using uCup.Services;

namespace uCup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IUCupProxy _uCupProxy;
        private readonly string _nfcRegexp = "^[A-F0-9]{8}$";
        private readonly IHealthCheckService _healthCheckService;

        public VendorController(IUCupProxy uCupProxy, IHealthCheckService healthCheckService)
        {
            _uCupProxy = uCupProxy;
            _healthCheckService = healthCheckService;
        }
        
        [HttpPost("Alive")]
        public async Task<bool> Alive(LiveRequest request)
        {
            if (request.RequestTime == default(DateTime))
            {
                request.RequestTime = DateTime.UtcNow.AddHours(8);
            }
            
            return await _healthCheckService.UpdateCache(request);
        }

        [HttpGet("GetToken")]
        public async Task<string> GetToken()
        {
            //var token = await _uCupProxy.GetTokenAsync(new Account("0900000000", "choosebetterbebetter"));
            var token = await _uCupProxy.GetTokenAsync(new Account("0922441389", "121537853"));
            return token;
        }

        [HttpPost("Register")]
        public async Task<VendorResponse> Register(RegisterRequest request)
        {
            var response = await _uCupProxy.Register(request);
            if (response.Success)
            {
                var rentRequest = new VendorRequest()
                {
                    UniqueId = request.UniqueId,
                    Password = request.Password,
                    Phone = request.Phone,
                    Provider = request.UniqueId.Length == 8 ? "NFC" : "Normal",
                    Type = "uCup"
                };

                await _uCupProxy.Rent(rentRequest);
            }
            
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
                    ErrorCode = response.ErrorCode,
                    Message = response.Result
                };
            }

            return new VendorResponse()
            {
                ErrorCode = response.ErrorCode,
                Message = response.Result
            };
        }
        
        [HttpPost("RentalStatus")]
        public async Task<VendorResponse> RentalStatus(RentalStatusRequest request)
        {
            // if (!Regex.IsMatch(request.UniqueId, NFC_regexp))
            // {
            //     return new VendorResponse()
            //     {
            //         ErrorCode = 500,
            //         Message = "Wrong NFC Id"
            //     };
            // }

            var response = await _uCupProxy.RentalStatus(request);

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
    }

    public class RentalStatusRequest
    {
        public string Phone { get; set; }
        public string Password { get; set; }
        public string NTUStudentId { get; set; }
        public string UniqueId { get; set; }
    }
}