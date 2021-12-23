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
        [HttpPost("Login")]

        public async Task<string> Login()
        {
            var formDataContent = new MultipartFormDataContent();
            formDataContent.Add(new StringContent("0900000000"), "phone");
            formDataContent.Add(new StringContent("choosebetterbebetter"), "password");
            var postAsync = await new HttpClient().PostAsync("https://ucup-dev.herokuapp.com/api/stores/login", formDataContent);
            postAsync.EnsureSuccessStatusCode();
            var readAsStringAsync = await postAsync.Content.ReadAsStringAsync();
            return readAsStringAsync;
        }

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
