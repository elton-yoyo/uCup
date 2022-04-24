using Newtonsoft.Json;

namespace uCup.Models
{
    public class Account
    {
        public Account(string phone, string password)
        {
            Phone = phone;
            Password = password;
        }

        [JsonProperty("phone")]
        public string Phone { get; private set; }
        [JsonProperty("password")]
        public string Password { get; private set; }
    }
}