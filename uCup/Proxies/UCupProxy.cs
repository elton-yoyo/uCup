using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using uCup.Controllers;
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
                    { new KeyValuePair<string, string>("phone", account.Phone) },
                    { new KeyValuePair<string, string>("password", account.Password) },
                };

                var formDataContent = new FormUrlEncodedContent(nameValueCollection);
                var data = await PostAsync<LoginResponse>(formDataContent, "stores/login");

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                    TimeSpan.FromDays(1));
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
            //response.EnsureSuccessStatusCode();
            var data = JsonConvert.DeserializeObject<TReturn>(await response.Content.ReadAsStringAsync());
            return data;
        }

        public async Task<RecordResponse> Return(VendorRequest recordRequest)
        {
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
            {
                { new KeyValuePair<string, string>("user_id", recordRequest.UniqueId) },
                { new KeyValuePair<string, string>("provider", recordRequest.Provider) },
                { new KeyValuePair<string, string>("cup_type", recordRequest.Type) },
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await GetTokenAsync(new Account(recordRequest.Phone, recordRequest.Password)));
            var formDataContent = new FormUrlEncodedContent(nameValueCollection);
            return await PostAsync<RecordResponse>(formDataContent, "record/do_return");
        }

        public async Task<RecordResponse> Rent(VendorRequest recordRequest)
        {
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
            {
                { new KeyValuePair<string, string> ("user_id", recordRequest.UniqueId) },
                { new KeyValuePair<string, string> ("provider", recordRequest.Provider) },
                { new KeyValuePair<string, string> ("cup_type", recordRequest.Type) },
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await GetTokenAsync(new Account(recordRequest.Phone, recordRequest.Password)));
            var formDataContent = new FormUrlEncodedContent(nameValueCollection);
            return await PostAsync<RecordResponse>(formDataContent, "record/do_rent");
        }

        public async Task<RecordResponse> Register(RegisterRequest request)
        {
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
            {
                { new KeyValuePair<string, string> ("ntu_id", request.NTUStudentId) },
                { new KeyValuePair<string, string> ("nfc_id", request.UniqueId) },
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await GetTokenAsync(new Account(request.Phone, request.Password)));
            var formDataContent = new FormUrlEncodedContent(nameValueCollection);
            return await PostAsync<RecordResponse>(formDataContent, "users/bind_ntu_nfc");
        }
    }
}