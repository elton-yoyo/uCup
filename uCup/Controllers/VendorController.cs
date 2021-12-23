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
                await _uCupProxy.GetTokenAsync(new Account("0900000000", "choosebetterbebetter"));
            return token;
        }

        [HttpPost("Rent")]
        public async Task<VendorResponse> Rent(VendorRequest request)
        {
            var token =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOjE2NDAyNjk3NTUsIm5iZiI6MTY0MDI2OTc1NSwianRpIjoiNzlmNWUwZGMtMzU5My00MWMwLThiMDMtNGJiYzdlMjM3MmNhIiwiZXhwIjoxNjQwODc0NTU1LCJpZGVudGl0eSI6eyJyb2xlIjoic3RvcmUiLCJpZCI6MTEsImFjdGl2ZSI6dHJ1ZSwic3RhdGUiOjAsImNyZWF0ZV90aW1lIjpbIjIwMjAtMDQtMTkiLCIwMzozMTo1NyJdLCJwaG9uZSI6IjA5MDAwMDAwMDAiLCJzdG9yZV9uYW1lIjoiXHU2NzZmXHU3Mjc5XHU3NmY0XHU3MWRmXHU1ZTk3Iiwic3RvcmVfdHlwZSI6InN0b3JlIiwiY3VwX251bSI6eyJpZCI6MywiYWN0aXZlIjp0cnVlLCJzdGF0ZSI6MCwiY3JlYXRlX3RpbWUiOlsiMjAyMC0wOS0xMiIsIjAwOjQyOjM3Il0sInN0b3JlX25hbWUiOiJcdTY3NmZcdTcyNzlcdTc2ZjRcdTcxZGZcdTVlOTciLCJjbGVhbl9udW1iZXIiOjk5MjYsImRpcnR5X251bWJlciI6MTYzLCJ1cGRhdGVfdGltZSI6WyIyMDIxLTExLTE4IiwiMTU6NTk6MjciXX0sImFkZHJlc3MiOiIiLCJsb25naXR1ZGUiOm51bGwsImxhdGl0dWRlIjpudWxsLCJlbWFpbCI6IiIsImltYWdlX3VybCI6Im51bGwiLCJpbmZvIjpudWxsLCJ3ZWJzaXRlIjoiIiwidXBkYXRlX3RpbWUiOlsiMjAyMS0wNC0xOCIsIjIzOjE4OjI3Il19LCJmcmVzaCI6ZmFsc2UsInR5cGUiOiJhY2Nlc3MifQ.Ua9_qzfQHIC7v6wDHpjBf68I8m0jIa9G1Wgw2YEhpow";
            var input = new PlatformRequest()
            {
                UniqueId = request.UniqueId,
                MerchantCode = request.MerchantCode,
                Time = DateTime.UtcNow.AddHours(8)
            };

            var client = new HttpClient();

            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
            {
                { new KeyValuePair<string, string>("user_id", request.UniqueId) },
                { new KeyValuePair<string, string>("provider", "NFC") },
                { new KeyValuePair<string, string>("cup_type", "uCup") },
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var a = new FormUrlEncodedContent(nameValueCollection);
            var test = await client.PostAsync("https://ucup-dev.herokuapp.com/api/record/do_rent", a);

            VenderCache.InsertCache(input);
            return new VendorResponse();
        }

        [HttpPost("Return")]
        public async Task<VendorResponse> Return(VendorRequest request)
        {
            var response = await _uCupProxy.Return(new RecordRequest(request.UniqueId, "NFC", "uCup"), new Account("0900000000", "choosebetterbebetter"));
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