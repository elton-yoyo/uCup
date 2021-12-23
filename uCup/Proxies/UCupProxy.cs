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

        public string GetToken(string account, string password)
        {
            if (!_tokenCache.TryGetValue(account, out string token))
            {
                var formDataContent = new MultipartFormDataContent();
                formDataContent.Add(new StringContent(account), "phone");
                formDataContent.Add(new StringContent(password), "password");
                _httpClient.PostAsync("/stores/login", formDataContent);
                var loginResponse = new LoginResponse()
                {
                    Success = true,
                    Token = "token"
                };
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                    TimeSpan.FromDays(7));
                _tokenCache.Set(account, loginResponse.Token, cacheEntryOptions);
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