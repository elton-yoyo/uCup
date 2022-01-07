using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using uCup.Controllers;
using uCup.Models;
using Google.Cloud.Logging.V2;
using Google.Cloud.Logging.Type;
using Google.Api;
using Google.Api.Gax.Grpc;
using Grpc.Core;

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
                var response = await _httpClient.PostAsync("stores/login", formDataContent);
                var data1 = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
                var data = data1;

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
            var response = await _httpClient.PostAsync("record/do_return", formDataContent);
            if (response.IsSuccessStatusCode)
            {
                return new RecordResponse()
                {
                    Success = true,
                    Result = "Success"
                };
            }

            var data = JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
            return data;
        }

        public async Task<RecordResponse> Rent(VendorRequest recordRequest)
        {
            WriteLogEntry("1", "Hello World");
            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
            {
                { new KeyValuePair<string, string> ("user_id", recordRequest.UniqueId) },
                { new KeyValuePair<string, string> ("provider", recordRequest.Provider) },
                { new KeyValuePair<string, string> ("cup_type", recordRequest.Type) },
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                await GetTokenAsync(new Account(recordRequest.Phone, recordRequest.Password)));
            var formDataContent = new FormUrlEncodedContent(nameValueCollection);
            var response = await _httpClient.PostAsync("record/do_rent", formDataContent);
            if (response.IsSuccessStatusCode)
            {
                return new RecordResponse()
                {
                    ErrorCode=0,
                    Success = true,
                    Result = "Success"
                };
            }

            var data = JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
            return data;
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
            var response = await _httpClient.PostAsync("users/bind_ntu_nfc", formDataContent);
            if (response.IsSuccessStatusCode)
            {
                return new RecordResponse()
                {
                    Success = true,
                    Result = "Success"
                };
            }

            var data = JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
            return data;
        }

        private readonly CallSettings _retryAWhile = CallSettings.FromRetry(
            RetrySettings.FromExponentialBackoff(
                maxAttempts: 15,
                initialBackoff: TimeSpan.FromSeconds(3),
                maxBackoff: TimeSpan.FromSeconds(12),
                backoffMultiplier: 2.0,
                retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Internal, StatusCode.DeadlineExceeded)));

        private void WriteLogEntry(string logId, string message)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName("ucup-335109", logId);
            LogEntry logEntry = new LogEntry
            {
                LogNameAsLogName = logName,
                Severity = LogSeverity.Info,
                TextPayload = $"{typeof(UCupProxy).FullName} - {message}"
            };
            MonitoredResource resource = new MonitoredResource { Type = "global" };
            IDictionary<string, string> entryLabels = new Dictionary<string, string>
            {
                { "size", "large" },
                { "color", "red" }
            };
            client.WriteLogEntries(logName, resource, entryLabels,
                new[] { logEntry }, _retryAWhile);
            Console.WriteLine($"Created log entry in log-id: {logId}.");
        }
    }
}