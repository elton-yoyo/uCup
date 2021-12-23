using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace uCup.Proxy
{
    public class UCupProxy : IUCupProxy
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _tokenCache;

        public UCupProxy(IMemoryCache tokenCache)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://ucup-dev.herokuapp.com/api/");
            _tokenCache = tokenCache;
        }

        public async Task<string> GetTokenAsync(string account, string password)
        {
            if (!_tokenCache.TryGetValue(account, out string token))
            {
                var formDataContent = new MultipartFormDataContent();
                formDataContent.Add(new StringContent(account), "phone");
                formDataContent.Add(new StringContent(password), "password");
                var response = await _httpClient.PostAsync("stores/login", formDataContent);
                response.EnsureSuccessStatusCode();
                var data = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                    TimeSpan.FromDays(7));
                if (data != null)
                {
                    token = data.Token;
                    _tokenCache.Set(account, data.Token, cacheEntryOptions);
                }
            }

            return token;
        }
    }

    public interface IUCupProxy
    {
        public Task<string> GetTokenAsync(string account, string password);
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
    }
}