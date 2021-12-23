using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace uCup.Models
{
    public class VendorRequest
    {
        [Required]
        public string UniqueId { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public string Provider { get; set; } = "NFC";
        public string Type { get; set; } = "uCup";
    }
}
