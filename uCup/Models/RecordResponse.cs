using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace uCup.Models
{
    public class RecordResponse
    {
        
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }
    }
}