using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            _httpClient.BaseAddress = new Uri("https://better-u-cup.herokuapp.com/api/");
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
                var data = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                    TimeSpan.FromDays(1));
                if (data != null && data.Token != null)
                {
                    token = data.Token;
                    _tokenCache.Set(account.Phone, data.Token, cacheEntryOptions);
                }
            }

            return token;
        }

        public async Task<RecordResponse> Return(VendorRequest recordRequest)
        {
            try
            {
                WriteLogEntry("Return", $"Return UniqueId: {recordRequest.UniqueId}" +
                                        $", Provider: {recordRequest.Provider}" +
                                        $", Type: {recordRequest.Type}" +
                                        $", From: {recordRequest.Phone}", LogSeverity.Info);

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

                try
                {
                    var tmp = await response.Content.ReadAsStringAsync();
                    WriteLogEntry("Return", "Get Return Response, result: " + tmp, LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    WriteLogEntry("Return", ex.ToString(), LogSeverity.Error);
                }

                if (response.IsSuccessStatusCode)
                {
                    return new RecordResponse()
                    {
                        Success = true,
                        Result = "Success",
                        ErrorCode = 101
                    };
                }

                var data = JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
                WriteLogEntry("Return", $"Return Response got error, msg = {data.Result}", LogSeverity.Info);
                return data;
            }
            catch (Exception ex)
            {
                WriteLogEntry("Return", $"Doing Return Exception: {ex}", LogSeverity.Error);
                throw new Exception();
            }
        }

        public async Task<RecordResponse> Rent(VendorRequest recordRequest)
        {
            try
            {
                WriteLogEntry("Rent", $"Rent UniqueId: {recordRequest.UniqueId}" +
                                      $", Provider: {recordRequest.Provider}" +
                                      $", Type: {recordRequest.Type}" +
                                      $", From: {recordRequest.Phone}", LogSeverity.Info);

                IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
                {
                    { new KeyValuePair<string, string>("user_id", recordRequest.UniqueId) },
                    { new KeyValuePair<string, string>("provider", recordRequest.Provider) },
                    { new KeyValuePair<string, string>("cup_type", recordRequest.Type) },
                };

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    await GetTokenAsync(new Account(recordRequest.Phone, recordRequest.Password)));
                var formDataContent = new FormUrlEncodedContent(nameValueCollection);
                var response = await _httpClient.PostAsync("record/do_rent", formDataContent);

                try
                {
                    var tmp = await response.Content.ReadAsStringAsync();
                    WriteLogEntry("Rent", "Get Rent Response, result: " + tmp, LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    WriteLogEntry("Rent", ex.ToString(), LogSeverity.Error);
                }

                if (response.IsSuccessStatusCode)
                {
                    return new RecordResponse()
                    {
                        ErrorCode = 0,
                        Success = true,
                        Result = "Success"
                    };
                }

                var data = JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
                WriteLogEntry("Rent", $"Rent Response got error, msg = {data.Result}", LogSeverity.Info);
                return data;
            }
            catch (Exception ex)
            {
                WriteLogEntry("Rent", $"Doing Rent Exception: {ex}", LogSeverity.Error);
                throw new Exception();
            }
        }

        public async Task<RecordResponse> Register(RegisterRequest request)
        {
            try
            {
                WriteLogEntry("Register", $"Register, ntu_id: {request.NTUStudentId}" +
                                          $", nfc_id: {request.UniqueId}" +
                                          $", From: {request.Phone}", LogSeverity.Info);
                IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>
                {
                    {
                        new KeyValuePair<string, string>("ntu_id",
                            request.NTUStudentId.Remove(request.NTUStudentId.Length - 1))
                    },
                    { new KeyValuePair<string, string>("nfc_id", request.UniqueId) },
                };

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    await GetTokenAsync(new Account(request.Phone, request.Password)));
                var formDataContent = new FormUrlEncodedContent(nameValueCollection);
                var response = await _httpClient.PostAsync("users/bind_ntu_nfc", formDataContent);
                WriteLogEntry("Register", "Get Register Response", LogSeverity.Info);
                if (response.IsSuccessStatusCode)
                {
                    return new RecordResponse()
                    {
                        Success = true,
                        Result = "Success"
                    };
                }

                var data = JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
                WriteLogEntry("Register", $"Register Response got error, msg = {data.Result}", LogSeverity.Info);
                return data;
            }
            catch (Exception ex)
            {
                WriteLogEntry("Register", $"Doing Register Exception: {ex}", LogSeverity.Error);
                throw new Exception();
            }
        }

        public async Task<RecordResponse> RentalStatus(RentalStatusRequest request)
        {
            try
            {
                WriteLogEntry("RentalStatus", $"RentalStatus UniqueId: {request.UniqueId}" +
                                              $", NTUStudentId: {request.NTUStudentId}" +
                                              $", From: {request.Phone}", LogSeverity.Info);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    await GetTokenAsync(new Account(request.Phone, request.Password)));

                var userId = string.IsNullOrEmpty(request.UniqueId)
                    ? request.NTUStudentId
                    : request.UniqueId;
                var response =
                    await _httpClient.GetAsync($"record/is_renting?user_id_from_provider={userId}&provider=Normal");

                try
                {
                    var tmp = await response.Content.ReadAsStringAsync();
                    WriteLogEntry("RentalStatus", "Get RentalStatus Response, result: " + tmp, LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    WriteLogEntry("RentalStatus", ex.ToString(), LogSeverity.Error);
                }

                if (response.IsSuccessStatusCode)
                {
                    var result =
                        JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
                    Boolean.TryParse(result.Result, out var isRenting);
                    return new RecordResponse()
                    {
                        ErrorCode = isRenting ? 1 : 0,
                        Success = true,
                        Result = result.Result
                    };
                }

                var data = JsonConvert.DeserializeObject<RecordResponse>(await response.Content.ReadAsStringAsync());
                WriteLogEntry("Rent", $"Rent Response got error, msg = {data.Result}", LogSeverity.Info);
                data.ErrorCode = 0;
                return data;
            }
            catch (Exception ex)
            {
                WriteLogEntry("Rent", $"Doing Rent Exception: {ex}", LogSeverity.Error);
                throw new Exception();
            }
        }

        private readonly CallSettings _retryAWhile = CallSettings.FromRetry(
            RetrySettings.FromExponentialBackoff(
                maxAttempts: 15,
                initialBackoff: TimeSpan.FromSeconds(3),
                maxBackoff: TimeSpan.FromSeconds(12),
                backoffMultiplier: 2.0,
                retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Internal, StatusCode.DeadlineExceeded)));

        public void WriteLogEntry(string logId, string message, LogSeverity severity)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName("ucup-335109", logId);
            LogEntry logEntry = new LogEntry
            {
                LogNameAsLogName = logName,
                Severity = severity,
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
            //Console.WriteLine($"Created log entry in log-id: {logId}.");
        }
    }
}