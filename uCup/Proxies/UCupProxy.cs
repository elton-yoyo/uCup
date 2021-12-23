using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;

namespace uCup.Proxy
{
    public class UCupProxy
    {
        private HttpClient _httpClient;
        private readonly IMemoryCache _tokenCache;

        public UCupProxy(HttpClient httpClient, IMemoryCache tokenCache)
        {
            _httpClient = httpClient;
            _tokenCache = tokenCache;
        }

        public string GetToken(string account,string password)
        {
            if (!_tokenCache.TryGetValue(account, out string token))
            {
                // _httpClient.PostAsync()
                var loginResponse = new LoginResponse()
                {
                    Success = true,
                    Token = "token"
                };
                _tokenCache.Set(account, loginResponse.Token);
            }

            return token;
        }
        
         
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
    }
}