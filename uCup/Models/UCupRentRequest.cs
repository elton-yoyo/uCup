using System;

namespace uCup.Models
{
    public class UCupRentRequest
    {
        public string UniqueId { get; set; }
        public string MerchantCode { get; set; }
        public DateTime Time { get; set; }
    }
}