using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using uCup.Models;

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

        public async Task<string> GetTokenAsync(Account account)
        {
            if (!_tokenCache.TryGetValue(account.Phone, out string token))
            {
                IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
                {
                    { new("phone", account.Phone) },
                    { new("password", account.Password) },
                };

                var formDataContent = new FormUrlEncodedContent(nameValueCollection);
                var data = await PostAsync<LoginResponse>(formDataContent, "stores/login");

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                    TimeSpan.FromDays(7));
                if (data != null)
                {
                    token = data.Token;
                    _tokenCache.Set(account.Phone, data.Token, cacheEntryOptions);
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

        public async Task<RecordResponse> Return(RecordRequest recordRequest, Account account)
        {
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
            {
                { new("user_id", recordRequest.Userid) },
                { new("provider", recordRequest.Provider) },
                { new("cup_type", recordRequest.Type) },
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await GetTokenAsync(account));
            var formDataContent = new FormUrlEncodedContent(nameValueCollection);
            return await PostAsync<RecordResponse>(formDataContent, "record/do_return");
        }
    }
}