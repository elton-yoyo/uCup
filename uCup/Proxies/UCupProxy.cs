using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace uCup.Proxies
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
                IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
                {
                    { new("phone", account) },
                    { new("password", password) },
                };

                var formDataContent = new FormUrlEncodedContent(nameValueCollection);
                var data = await PostAsync<LoginResponse>(formDataContent, "stores/login");

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

        private async Task<TReturn> PostAsync<TReturn>(FormUrlEncodedContent formDataContent, string path)
        {
            var response = await _httpClient.PostAsync(path, formDataContent);
            response.EnsureSuccessStatusCode();
            var data = JsonConvert.DeserializeObject<TReturn>(await response.Content.ReadAsStringAsync());
            return data;
        }

        public async Task<RecordResponse> Return(string userid, string provider, string type)
        {
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
            {
                { new("user_id", userid) },
                { new("provider", provider) },
                { new("cup_type", type) },
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await GetTokenAsync("0900000000", "choosebetterbebetter"));
            var formDataContent = new FormUrlEncodedContent(nameValueCollection);
            var test = await _httpClient.PostAsync("record/do_return", formDataContent);
            return await PostAsync<RecordResponse>(formDataContent, "stores/login");
        }
    }

    public class RecordResponse
    {
        public bool Success { get; set; }
        public string Result { get; set; }
    }

    public interface IUCupProxy
    {
        public Task<string> GetTokenAsync(string account, string password);
        Task<RecordResponse> Return(string userid, string provider, string type);
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
    }
}