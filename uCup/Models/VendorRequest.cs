using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uCup.Models
{
    public class VendorRequest
    {
        public string UniqueId { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Provider => "NFC";
        public string Type => "uCup";
    }
}
