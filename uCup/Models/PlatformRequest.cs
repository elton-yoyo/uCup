using System;

namespace uCup.Models
{
    public class PlatformRequest
    {
        public string UniqueId { get; set; }
        public string MerchantCode { get; set; }
        public DateTime Time { get; set; }
    }
}