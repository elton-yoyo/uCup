namespace uCup.Models
{
    public class RecordRequest
    {
        public RecordRequest(string userid, string provider, string type)
        {
            Userid = userid;
            Provider = provider;
            Type = type;
        }

        public string Userid { get; private set; }
        public string Provider { get; private set; }
        public string Type { get; private set; }
    }
}