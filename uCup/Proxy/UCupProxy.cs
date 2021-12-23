using System;
using System.Net.Http;

namespace uCup.Proxy
{
    public class UCupProxy
    {
        private HttpClient _httpClient;

        public UCupProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Login(string account,string password)
        {
            // _httpClient.PostAsync()
        }
    }
}