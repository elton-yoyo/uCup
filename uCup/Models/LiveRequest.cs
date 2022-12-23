using System;

namespace uCup.Models
{
    public class LiveRequest
    {
        public string MachineName { get; set; }
        public string Ip { get; set; }
        public bool Status { get; set; }
        public DateTime RequestTime { get; set; }
    }
}