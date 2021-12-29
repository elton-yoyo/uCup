using System.Text.Json.Serialization;

namespace uCup.Models
{
    public class RecordResponse
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        
        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }
    }
}